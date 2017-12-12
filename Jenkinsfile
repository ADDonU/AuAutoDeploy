#!groovy​

def Build(method) {
    sh """
                    /Applications/Unity/Unity.app/Contents/MacOS/Unity 
                        -batchmode 
                        -nographics    
                        -silent-crashes
                        -executeMethod BuildAutomation.${method}
                        -quit 
                        -logFile /dev/stdout
                """.replaceAll("[\r\n]+"," ")
}

pipeline {
    agent any
    options { 
        disableConcurrentBuilds() 
    }

    environment {
        PATH="$PATH:/usr/local/bin"
    }

    stages {

        stage ('Clean')
        {
            steps {
                dir("target") {
                    deleteDir();
                }
                dir("build") {
                    deleteDir();
                }
            }
        }

        
        stage ('Test') {
            steps {
                sh """
                    /Applications/Unity/Unity.app/Contents/MacOS/Unity 
                        -batchmode 
                        -nographics    
                        -silent-crashes
                        -runEditorTests
                        -editorTestsResultFile target/mac-32/result.xml
                        -logFile /dev/stdout
                """.replaceAll("[\r\n]+"," ")
            }
        }
        

/*
        stage ('Sonar Analysis') {
            environment {
                MONO_HOME = '/Library/Frameworks/Mono.framework/Versions/Current'
                DOTNET_HOME= '/usr/local/share/dotnet'
            }
            steps {
                sh """
                    /Applications/Unity/Unity.app/Contents/MacOS/Unity 
                        -batchmode 
                        -nographics    
                        -silent-crashes
                        -executeMethod ProjectExporter.ExportCSharpProject
                        -quit 
                        -logFile /dev/stdout
                """.replaceAll("[\r\n]+"," ")


                withSonarQubeEnv('Sonar') {

                    sh """
                        ${MONO_HOME}/Commands/mono 
                            /opt/sonar-scanner-msbuild/SonarQube.Scanner.MSBuild.exe begin 
                            /key:Cubica
                            /d:sonar.host.url=$SONAR_HOST_URL
                            /d:sonar.login=$SONAR_AUTH_TOKEN
                        """.replaceAll("[\r\n]+"," ")

                    //sh "${DOTNET_HOME}/dotnet restore Sonar.sln"
                    sh "${MONO_HOME}/Commands/msbuild /t:rebuild Sonar.sln"
                    sh """
                        ${MONO_HOME}/Commands/mono 
                            /opt/sonar-scanner-msbuild/SonarQube.Scanner.MSBuild.exe 
                            end
                            /d:sonar.login=$SONAR_AUTH_TOKEN
                        """.replaceAll("[\r\n]+"," ")

                    
                }
            }
        }
*/


        stage('Linux Server') {
            steps {
                script { Build("LinuxServer")}
                sh "docker build . -t ummorpg-server"
                sh "docker stop ummorpg || true"
                sh "docker rm ummorpg || true"
                sh """
                    docker run 
                        --name=ummorpg
                        --restart=always
                        -e MYSQL_ROOT_PASSWORD=mypwd 
                        -p 7777:7777/udp
                        -d ummorpg-server
                    """.replaceAll("[\r\n]+"," ")
            }
        }

        stage('Linux 32-bit') {
            steps {
                script { Build("Linux32")}
                dir ("build/linux-32") {
                    writeFile file:'info.txt', text:'Linux 32-bit'
                }
                sh "tar -cvf - -C target/linux-32 . | /usr/local/bin/gzip --rsyncable --best > build/linux-32/cubica.tar.gz"
                archive 'build/linux-32/Cubica.tar.gz'
            }
        }

        stage('Linux 64-bit') {
            steps {
                script { Build("Linux64")}
                dir ("build/linux-64") {
                    writeFile file:'info.txt', text:'Linux 64-bit'
                }
                sh "tar -cvf - -C target/linux-64 . | /usr/local/bin/gzip --rsyncable --best > build/linux-64/cubica.tar.gz"
                archive 'build/linux-64/Cubica.tar.gz'
            }
        }
        
        stage('Windows 32-bit') {
            steps {
                script { Build("Windows32")}
                dir ("build/win-32") {
                    writeFile file:'info.txt', text:'Windows 32-bit'
                }
                sh """
                    msi-packager
                        target/win-32
                        build/win-32/ummorpg.msi
                        --name uMMORPG
                        --version 0.1
                        --manufacturer xxxxx
                        --arch x86
                        --upgrade-code xxxxxxxxx
                        --icon xxxx.ico
                        --executable ummorpg.exe
                    """.replaceAll("[\r\n]+"," ")
                archive 'build/win-32/ummorpg.msi'
            }
        }

        stage('Windows 64-bit') {
            steps {
                script { Build("Windows64")}
                dir ("build/win-64") {
                    writeFile file:'info.txt', text:'Windows 64-bit'
                }
                sh """
                    msi-packager
                        target/win-64
                        build/win-64/Cubica.msi
                        --name Cubica
                        --version 0.1
                        --manufacturer Mindblocks
                        --arch x64
                        --upgrade-code 25242f06-3fc1-4eee-bb82-2db53c18ea42
                        --icon Cubica.ico
                        --executable Cubica.exe
                    """.replaceAll("[\r\n]+"," ")
                archive 'build/win-64/Cubica.msi'
            }
        }

        stage('MacOS 32-bit') {
            steps {
                script { Build("MacOSX32")}
                dir ("build/mac-32") {
                    writeFile file:'info.txt', text:'Mac OS 32-bit'
                }
                sh "appdmg mac32build.json build/mac-32/Cubica.dmg"
                archive 'build/mac-32/Cubica.dmg'
            }
        }

        stage('MacOS 64-bit') {
            steps {
                script { Build("MacOSX64")}
                dir ("build/mac-64") {
                    writeFile file:'info.txt', text:'Mac OS 64-bit'
                }
                sh "appdmg mac64build.json build/mac-64/Cubica.dmg"
                archive 'build/mac-64/Cubica.dmg'
            }
        }

       


        stage('Publish') {
            steps {
                sh "rsync -av --progress -e \"ssh -p 18765\" -r build/ blockst0@blockstory.net:cubica.net/wp-content/build/"

                //sh "scp -P18765 -v -r build/* blockst0@blockstory.net:cubica.net/wp-content/build/"
            }
        }
    }
 
    post {
        always {
            nunit testResultsPattern: 'target/mac-32/result.xml'
        }
    }
    /* insert Declarative Pipeline here */
}
