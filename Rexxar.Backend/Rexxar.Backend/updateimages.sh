docker stop rexxarbackend_rexxar.chatapi_1 rexxarbackend_rexxar.identity_1 
docker rm rexxarbackend_rexxar.chatapi_1 rexxarbackend_rexxar.identity_1 
docker rmi rexxarbackend_rexxar.chatapi rexxarbackend_rexxar.identity
docker-compose -f docker-compose.yml up -d
