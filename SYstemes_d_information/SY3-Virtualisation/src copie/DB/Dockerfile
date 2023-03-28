# MariaDB 10.3 with SSH
# Pull the mariadb latest image
FROM mariadb:latest
# List all the packages that we want to install
ENV PACKAGES openssh-server openssh-client
ENV MARIADB_ROOT_PASSWORD="Secret+++++++++"
ENV MYSQL_DATABASE="QuizDB"

COPY ./init_db.sql /docker-entrypoint-initdb.d/init_db.sql