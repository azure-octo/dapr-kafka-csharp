# Producer and Consumer examples using Dapr Pubsub

Instructions to run the sample

## Pre-requisites

1. Clone the git repository to download the sample
```
git clone https://github.com/azure-octo/dapr-kafka-csharp.git
```
2. Install docker
3. Install .NET

## Running locally

### Run Kafka Docker Container Locally

In order to run the Kafka bindings sample locally, you will run the [Kafka broker server](https://github.com/wurstmeister/kafka-docker) in a docker container on your machine.

1. Run `docker-compose -f ./docker-compose-single-kafka.yml up -d` to run the container locally
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

## Run in Kubernetes cluster

### Setting up a Kafka in Kubernetes

1. Make sure that you installed helm tiller on your cluster
2. Install Kafka via [incubator/kafka helm chart](https://github.com/helm/charts/tree/master/incubator/kafka)
```
$ helm repo add incubator http://storage.googleapis.com/kubernetes-charts-incubator
$ helm repo update
$ kubectl create ns kafka
$ helm install dapr-kafka incubator/kafka -f ./kafka-non-persistence.yaml
```

3. Wait until kafka pods are running
```
$ kubectl get pods -w
NAME                     READY   STATUS    RESTARTS   AGE
dapr-kafka-0             1/1     Running   0          2m7s
dapr-kafka-zookeeper-0   1/1     Running   0          2m57s
dapr-kafka-zookeeper-1   1/1     Running   0          2m13s
dapr-kafka-zookeeper-2   1/1     Running   0          109s
```

4. Run Dapr in Kubernetes environment
```
dapr init --kubernetes
```

5. Deploy the producer and consumer applications to Kubernetes
```
kubectl apply -f ./deploy/producer.yaml
kubectl apply -f ./deploy/consumer.yaml
```

6. Check the logs from producer and consumer:
```
kubectl logs -f producer-567bf6fbfd-42zjf producer
kubectl logs -f consumer-bcd4bb7b4-k2pvt consumer
```
