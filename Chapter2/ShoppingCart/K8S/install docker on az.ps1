kubectl create clusterrolebinding kubernetes-dashboard --clusterrole=cluster-admin --serviceaccount=kube-system:kubernetes-dashboard
az aks browse --resource-group MicroservicesInDotnet --name MicroservicesInDotnetAKSCluster

docker tag shopping-cart shoppingcartnk.azurecr.io/shopping-cart:1.0.0
az acr login --name shoppingcartnk
docker push shoppingcartnk.azurecr.io/shopping-cart:1.0.0
kubectl apply -f shopping-cart-azure.yaml

#After End Of tests
az group delete --name MicroservicesInDotnet --yes --no-wait
