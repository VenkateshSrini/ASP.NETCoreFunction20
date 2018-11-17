REM docker build -t azurefuncdocker .
docker build -t venkateshsrini3/azurefunctiondocker .
docker run -p 8080:80 azurefuncdocker
REM http://localHost:8080
REM docker rm  <CONTAINER ID>
REM docker rmi  <IMAGE ID>
REM kubectl run azurefuncsample1 --image=venkateshsrini3/azurefuncdocker --port=80
REM kubectl expose deployment azurefuncsample1 --type=LoadBalancer --target-port=80 --port=3001
REM kubectl autoscale deploy azurefuncsample1 --cpu-percent=20 --max=5 --min=1