#!/bin/sh

export SPACE_NAME="PROJECT_NAME"
export BACKUP_LOCATION="$HOME/backup"
export BACKUP_NAME=$(date +%y%m%d_%H%M%S).agz
export HOST="mongodb"
export PORT="21271"
export USER_NAME="user"
export PASS="password"
export CONTAINER_NAME="infras_mongodb_1"

if [ -d "$BACKUP_LOCATION" ]
then
        echo "$BACKUP_LOCATION is exist!"
else
        echo "$BACKUP_LOCATION isn't exist!"
        mkdir $BACKUP_LOCATION
        echo "$BACKUP_LOCATION is created!"
fi

docker exec -t $CONTAINER_NAME mongodump --host=$HOST --port=$PORT --username=$USER_NAME --password=$PASS --gzip --archive=data/db/$SPACE_NAME-$BACKUP_NAME
docker cp $CONTAINER_NAME:/data/db/$SPACE_NAME-$BACKUP_NAME $BACKUP_LOCATION
aws s3 cp $BACKUP_LOCATION/$SPACE_NAME-$BACKUP_NAME s3://s3-bucket/folder/

echo 'Backup successfully!'

===
0 0 * * * /bin/bash ~/bk.sh