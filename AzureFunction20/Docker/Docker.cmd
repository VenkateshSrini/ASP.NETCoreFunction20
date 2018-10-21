docker build -t azurefuncdocker .
docker run -p 8080:80 azurefuncdocker
REM http://localHost:8080
REM docker rm  <CONTAINER ID>
REM docker rmi  <IMAGE ID>