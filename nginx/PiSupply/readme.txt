run sudo nano /etc/rc.local

and then add in the following code:

printf "Starting soft shutdown...\n"
python /home/pi/PiSupply/softshut.py &
printf "Starded\n"

before the line that says:

exit 0

save and exit.