docker build -t jogging.api .
docker run --rm -it -p 5000:80 -e ASPNETCORE_ENVIRONMENT=Development -e ASPNETCORE_URLS="http://+" --name jogging.api -d jogging.api
