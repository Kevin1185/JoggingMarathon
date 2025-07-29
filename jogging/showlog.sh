name="jogging_jogging.api_1"
echo $name
for container_id in $(docker ps --filter "name=$name" -q); 
do 
	docker logs --follow "$container_id"; 
done
