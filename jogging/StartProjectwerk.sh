#!/bin/bash


if [ "$1" != "" ]
then
	D="${0%/*}"
	if [ "$D" == "." -o "$D" == "$0" ]
	then
		D="${PWD##*/}"
	else
		if [ ! -d "$D" ]
		then
			echo "Project '$D' not found"
			exit 3
		fi
		cd "$D"
	fi

	
	if [ ! -f ${D}*.tar ]
	then
		echo "No tar file found in '$D'"
		exit 3
	fi
	
	if [ ! -f "docker-compose.yml" ]
	then
		echo "No docker-compose.yml file found in '$D'"
		exit 3
	fi
fi

case "$1" in
	up)
		all=`docker image list -q`
		if [ "$all" == "" ]
		then
	       		echo "No images found, load docker images first"
			exit 2
		fi
		rm -f nohup.out
		nohup docker-compose -p "$D" up &
		;;
	down)
		docker-compose -p "$D" down
		;;
	start)
		echo "Perhaps you should use up?"
		exit 2
		;;
	stop)
		echo "Perhaps you should use down?"
		exit 2
		;;
	load)
		docker load -i ${D}*.tar
		;;
	unload)
		# check if things are running
		all=`docker ps -a -q`
		if [ "$all" != "" ]
		then
			# stop running images
			docker stop $all
			# remove containers
			docker rm $all
		fi
		all=`docker image list -q`
		if [ "$all" != "" ]
		then
	       		docker image rm $all
		fi
		;;
	*)
		echo "$0 [up|down|start|stop|load]"
		exit 2
		;;
esac
