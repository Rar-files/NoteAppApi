#!/bin/sh
api_image_name=note-app-api

start() {
    echo Backend stack starting...
    docker-compose up -d
    echo Backend stack started
}

build() {
    isStarted=false
    if [ "$(docker ps -a | grep note-app-api | wc -l)" -gt 0 ]; then
        isStarted=true
        echo Backend stack is running.
        down
    fi

    if docker image inspect $api_image_name >/dev/null 2>&1; then
        echo API image exists.
        clean
    fi

    echo Build new API image...
    docker build -t note-app-api -f Dockerfile .
    echo Image build successful.

    if [ "$isStarted" = true ]; then
        start
    fi
}

down() {
    echo Backend stack stopping...
    docker-compose down
    echo Backend stack stopped.
}

clean() {
    echo Removing old API image...
    docker rmi note-app-api
    echo Old API image removed.
}

help() {
    echo "Usage: $0 [OPTIONS]"
    echo "Options:"
    echo " -b, --build   Start backend stack with rebuilding API image"
    echo " -c, --clean   Remove old files"
    echo " -d, --down      Stop and remove backend stack"
    echo " -h, --help      Display this help message"
    echo " -s, --start     Start backend stack"
}


handle_options() {
    while [ $# -gt 0 ]; do
        case "$1" in
            -b | --build)
                build
                ;;
            -c | --clean)
                clean
                ;;
            -s | --start)
                start
                ;;
            -d | --down)
                down
                ;;
            -h | --help)
                help
                ;;
            *)
                help
                ;;
        esac
        shift
    done
}

handle_options "$@"

exit 0