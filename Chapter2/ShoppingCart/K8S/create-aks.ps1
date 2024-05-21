az group create --name MicroservicesInDotnet --location northeurope
az acr create --resource-group MicroservicesInDotnet --name shoppingcartnk --sku Basic
az aks create --resource-group MicroservicesInDotnet --name MicroservicesInDotnetAKSCluster --node-count 1 --enable-addons monitoring --generate-ssh-keys --attach-acr shoppingcartnk 
az aks get-credentials --resource-group MicroservicesInDotnet --name MicroservicesInDotnetAKSCluster
kubectl get nodes