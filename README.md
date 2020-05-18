# Producer and Consumer examples using Dapr Pubsub

## Pre-requisites

1. Install [Docker](https://www.docker.com/products/docker-desktop)
2. Install [.Net Core SDK 3.1](https://dotnet.microsoft.com/download)
3. Clone the sample repo

```
git clone https://github.com/azure-octo/dapr-kafka-csharp.git
```

## Running locally

### Install Dapr in standalone mode

1. [Install Dapr CLI](https://github.com/dapr/docs/blob/master/getting-started/environment-setup.md#installing-dapr-cli)
2. [Install Dapr in standalone mode](https://github.com/dapr/docs/blob/master/getting-started/environment-setup.md#installing-dapr-in-standalone-mode)

### Run Kafka Docker Container Locally

In order to run the Kafka bindings sample locally, you will run the [Kafka broker server](https://github.com/wurstmeister/kafka-docker) in a docker container on your machine. Make sure docker is running in Linux mode.

1. Run `docker-compose -f ./docker-compose-kafka.yml up -d` to run the container locally
2. Run `docker ps` to see the container running locally: 

```bash
CONTAINER ID        IMAGE                           COMMAND                  CREATED             STATUS              PORTS                                                NAMES
aaa142160487        wurstmeister/zookeeper:latest   "/bin/sh -c '/usr/sb…"   2 minutes ago       Up 2 minutes        22/tcp, 2888/tcp, 3888/tcp, 0.0.0.0:2181->2181/tcp   dapr-kafka-csharp_zookeeper_1
0e3908026eda        wurstmeister/kafka:latest       "start-kafka.sh"         2 minutes ago       Up 2 minutes        0.0.0.0:9092->9092/tcp                               dapr-kafka-csharp_kafka_1
c0c3ca47c0ad        daprio/dapr                     "./placement"            3 days ago          Up 32 hours         0.0.0.0:50005->50005/tcp                             dapr_placement
c8eec02b4e5d        redis                           "docker-entrypoint.s…"   3 days ago          Up 32 hours         0.0.0.0:6379->6379/tcp                               dapr_redis
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
docker-compose -f ./docker-compose-kafka.yml down
```

## Run in Kubernetes cluster

### Install Dapr on Kubernetes

```
$ dapr init -k
⌛  Making the jump to hyperspace...
ℹ️  Note: this installation is recommended for testing purposes. For production environments, please use Helm 

✅  Deploying the Dapr control plane to your cluster...
✅  Success! Dapr has been installed. To verify, run 'kubectl get pods -w' or 'dapr status -k' in your terminal. To get started, go here: https://aka.ms/dapr-getting-started

$ kubectl get pods -w
NAME                                     READY   STATUS    RESTARTS   AGE
dapr-operator-6bdc6f95c6-g67p2           1/1     Running   0          37s
dapr-placement-fb75fb85-k6m7d            1/1     Running   0          37s
dapr-sentry-6f796dd4cb-rh9qx             1/1     Running   0          37s
dapr-sidecar-injector-7bc488df76-jg6fw   1/1     Running   0          37s
```

> For more detail, refer to [Dapr in Kubernetes environment](https://github.com/dapr/docs/blob/master/getting-started/environment-setup.md#installing-dapr-on-a-kubernetes-cluster) for more detail.
> For helm users, please refer to [this](https://github.com/dapr/docs/blob/master/getting-started/environment-setup.md#using-helm-advanced).

### Setting up a Kafka in Kubernetes

1. Install Kafka via [incubator/kafka helm chart](https://github.com/helm/charts/tree/master/incubator/kafka)
```
$ helm repo add incubator http://storage.googleapis.com/kubernetes-charts-incubator
$ helm repo update
$ kubectl create ns kafka
$ helm repo add azure-marketplace https://marketplace.azurecr.io/helm/v1/repo
$ helm install dapr-kafka azure-marketplace/kafka -n kafka -f ./kafka-non-persistence.yaml
```

2. Wait until kafka pods are running
```
$ kubectl get pods -n kafka -w
NAME                     READY   STATUS    RESTARTS   AGE
dapr-kafka-0             1/1     Running   0          2m7s
dapr-kafka-zookeeper-0   1/1     Running   0          2m57s
dapr-kafka-zookeeper-1   1/1     Running   0          2m13s
dapr-kafka-zookeeper-2   1/1     Running   0          109s
```

3. Deploy the producer and consumer applications to Kubernetes
```
kubectl apply -f ./deploy/kafka-pubsub.yaml
kubectl apply -f ./deploy/producer.yaml
kubectl apply -f ./deploy/consumer.yaml
```

4. Check the logs from producer and consumer:
```
kubectl logs -f -l app=producer -c producer
kubectl logs -f -l app=consumer -c consumer
```

## Build and push docker image to your docker registry

1. Create your docker hub account or use your own docker registry

2. Build Docker images.
```sh
docker build -t [docker_registry]/consumer:latest -f ./consumer/Dockerfile .
docker build -t [docker_registry]/producer:latest -f ./producer/Dockerfile .
```

3. Push Docker images.
```sh
docker push [docker_registry]/consumer:latest
docker push [docker_registry]/producer:latest
```

4. Update image names
  * Update imagename to [docker_registry]/consumer:latest in [deploy/consumer.yaml](https://github.com/azure-octo/dapr-kafka-csharp/blob/master/deploy/consumer.yaml#L39)
  * Update imagename to [docker_registry]/producer:latest in [deploy/producer.yaml](https://github.com/azure-octo/dapr-kafka-csharp/blob/master/deploy/producer.yaml#L23)

## Cleanup

1. Stop the applications
```
kubectl delete -f ./deploy/kafka-pubsub.yaml
kubectl delete -f ./deploy/consumer.yaml
kubectl delete -f ./deploy/producer.yaml
```

2. Uninstall Kafka
```
helm uninstall dapr-kafka -n kafka
```

3. Uninstall Dapr
```
dapr uninstall -k
```
