echo "Making directory pitemp"
mkdir -m 755 /var/log/pitemp
chown -R pi:pi /var/log/pitemp
cp pitemp /etc/logrotate.d

