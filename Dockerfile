# to build
# sudo docker build . -t cubica

# to run
# sudo docker run -t -i cubica

# to explore
# sudo docker run --rm -it --entrypoint=/bin/sh cubica



FROM bitnami/minideb:latest


LABEL Description="This image is used to start the server" Vendor="xxxxx" Version="0.1"

RUN apt-get update && apt-get install -y \
    sqlite3 

ADD target/linux-server/ummorpg /ummorpg/ummorpg
ADD target/linux-server/ummorpg_Data /ummorpg/ummorpg_Data

EXPOSE 7777/udp

ENTRYPOINT /ummorpg/ummorpg -logfile -
