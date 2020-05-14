# Producer and Consumer examples using Dapr Pubsub

## Pre-requisites

1. Install [Docker](https://www.docker.com/products/docker-desktop)
2. Install [.Net Core SDK 3.1](https://dotnet.microsoft.com/download)
3. Install [Dapr CLI](https://github.com/dapr/cli)
4. Clone the sample repo

```
git clone https://github.com/azure-octo/dapr-kafka-csharp.git
```

## Running locally

### Run Kafka Docker Container Locally

In order to run the Kafka bindings sample locally, you will run the [Kafka broker server](https://github.com/wurstmeister/kafka-docker) in a docker container on your machine.

1. Run `docker-compose -f ./docker-compose-kafka.yml up -d` to run the container locally
2. Run `docker ps` to see the container running locally: 

```bash
342d3522ca14        kafka-docker_kafka                      "start-kafka.sh"         14 hours ago        Up About
a minute   0.0.0.0:9092->9092/tcp                               kafka-docker_kafka_1
0cd69dbe5e65        wurstmeister/zookeeper                  "/bin/sh -c '/usr/sb…"   8 days ago          Up About
a minute   22/tcp, 2888/tcp, 3888/tcp, 0.0.0.0:2181->2181/tcp   kafka-docker_zookeeper_1
```

### Run Consumer app

```
cd consumer
dapr run --app-id consumer --app-port 6000 dotnet run
```

### Run Producer app

```
cd producer
dapr run --app-id producer dotnet run
```

### Uninstall Kafka

```
Linux/MacOS:
    docker ps | grep kafka
Windows:
    docker ps | findstr kafka
docker stop [container ids for kafka]
```

## Run in Kubernetes cluster

### Setting up a Kafka in Kubernetes

1. Make sure that you installed helm tiller on your cluster
2. Install Kafka via [incubator/kafka helm chart](https://github.com/helm/charts/tree/master/incubator/kafka)
```
$ helm repo add incubator http://storage.googleapis.com/kubernetes-charts-incubator
$ helm repo update
$ kubectl create ns kafka
$ helm repo add azure-marketplace https://marketplace.azurecr.io/helm/v1/repo
$ helm install dapr-kafka azure-marketplace/kafka -n kafka -f ./kafka-non-persistence.yaml
```

1. Wait until kafka pods are running
```
$ kubectl get pods -n kafka -w
NAME                     READY   STATUS    RESTARTS   AGE
dapr-kafka-0             1/1     Running   0          2m7s
dapr-kafka-zookeeper-0   1/1     Running   0          2m57s
dapr-kafka-zookeeper-1   1/1     Running   0          2m13s
dapr-kafka-zookeeper-2   1/1     Running   0          109s
```

4. Install [Dapr in Kubernetes environment](https://github.com/dapr/docs/blob/master/getting-started/environment-setup.md#installing-dapr-on-a-kubernetes-cluster)


5. Deploy the producer and consumer applications to Kubernetes
```
kubectl apply -f ./deploy/kafka-pubsub.yaml
kubectl apply -f ./deploy/producer.yaml
kubectl apply -f ./deploy/consumer.yaml
```

6. Check the logs from producer and consumer:
```
kubectl logs -f producer-567bf6fbfd-42zjf producer
kubectl logs -f consumer-bcd4bb7b4-k2pvt consumer
```

## Build and push docker image to your docker registry

```sh
docker build -t [docker_registry]/consumer:latest -f ./consumer/Dockerfile .
docker build -t [docker_registry]/producer:latest -f ./producer/Dockerfile .

docker push [docker_registry]/consumer:latest
docker push [docker_registry]/producer:latest
```

update producer/consumer.yaml with the appropriate image name [docker_registry]/consumer:latest, [docker_registry]/producer:latest

## Cleanup
Stop the applications
```
kubectl delete -f ./deploy/kafka-pubsub.yaml
kubectl delete -f ./deploy/consumer.yaml
kubectl delete -f ./deploy/producer.yaml
```
Uninstall Kafka
```
helm uninstall dapr-kafka -n kafka
```
Uninstall Dapr
```
helm uninstall dapr -n dapr-system
```
