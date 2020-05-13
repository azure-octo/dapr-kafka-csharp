#!/bin/sh

docker build -t consumer:latest -f ./consumer/Dockerfile .
docker build -t producer:latest -f ./producer/Dockerfile .
