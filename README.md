# NoteApp backend

This repository contains the source code for a NoteApp backend. The application environment is fully automated and containerized using Docker.
Make sure Docker with Docker-compose is installed and running on your system before executing the script.

## Usage

To run stack, execute:
```
./service.sh --start
```
After changes in code, execute:
```
./service.sh --build
```
To stop stack, execute:
```
./service.sh --down
```
To clean after stack, execute:
```
./service.sh --clean
```

## Permissions
You might need to add permissions to execute this file, try executing:
```
chmod +x service.sh
```