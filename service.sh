#!/bin/sh
api_image_name=note-app-api
isRebuild=false

up() {
    build
    start
}

build() {
    file=app/appsettings.local.json

    if [ ! -e "$file" ] ; then
        echo Generate local settings file with app secret...
        key=$(head -c 32 /dev/random | base64)
        echo "{
    \"Secrets\": {
        \"JWTKey\": \"$key\"
    }
}" > "$file"
        echo "File \"$file\" with app secret generated."
    fi

    if docker image inspect $api_image_name >/dev/null 2>&1; then

        echo API image exists.
        
        if [ "$isRebuild" = false ]; then
            echo Use -r or --rebuild to force rebuild.
            return
        fi       

        rmi
    fi

    echo Build new API image...
    cd app
    docker build -t note-app-api -f Dockerfile .
    cd ..
    echo Image build successful.
}

down() {
    echo Backend stack stopping...
    docker-compose down
    echo Backend stack stopped.
}

start() {
    echo Backend stack starting...
    docker-compose up -d
    echo Backend stack started
}

rmi() {
    if [ "$(docker ps -a | grep note-app-api | wc -l)" -gt 0 ]; then
        echo Backend stack is running.
        down
    fi

    echo Removing old API image...
    docker rmi note-app-api
    echo Old API image removed.
}

clean() {
    echo "ATTENTION!: This command remove app image and DATABASE FILES"
    echo "Do you want to continue? (y/n)"
    read -r answer
    if [ "$answer" == "y" ]; then
        rmi
        rm -r ./postgres-data
        echo All stack files removed.
    else
        echo Operation aborted.
    fi
}

help() {
    echo ""
    echo "Usage: $0 [COMMAND] [OPTIONS]"
    echo ""
    echo "Commands:"
    echo " b, build             Build backend stack"
    echo " c, clean             Remove stack files (ATTENTION!: This command remove app image and DATABASE FILES)"
    echo " d, down              Stop and remove backend stack"
    echo " h, help              Display this help message"
    echo " rmi, remove-image    Remove old API image"
    echo " u, up                Start backend stack"
    echo ""
    echo "Options:"
    echo " -r, --rebuild    Force to rebuild api image during up or build, if old image exist"
}


handle_options() {
    first="$1"
    while [ $# -gt 1 ]; do
        case "$2" in
            -r | --rebuild)
                isRebuild=true
                ;;
            *)
                help
                ;;
        esac
        shift
    done

    case "$first" in
        b | build)
            build
            ;;
        c | clean)
            clean
            ;;
        u | up)
            up
            ;;
        d | down)
            down
            ;;
        rmi | remove-image)
            rmi
            ;;
        h | help)
            help
            ;;
        *)
            help
            ;;
    esac
}

handle_options "$@"

exit 0