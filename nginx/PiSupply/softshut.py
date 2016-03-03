# Import the modules to send commands to the system and access GPIO pins
from subprocess import call
import RPi.GPIO as gpio

# Define variables to store the pin numbers
soft_shutdown_pin = 13 # Default pin for Pi Supply is 7

# Define a function to run when an interrupt is called
def shutdown():
    # Cleanup GPIO
    gpio.cleanup()

    # Shutdown with halt and power off in 1 minute
    call(['shutdown', '-h','now'], shell=False)

# Set pin numbering to board numbering
gpio.setmode(gpio.BOARD)

# Setup the input Pin to wait on
gpio.setup(soft_shutdown_pin, gpio.IN)

gpio.wait_for_edge(soft_shutdown_pin, gpio.RISING) # Wait for input from button

# Run the shutdown function
shutdown()
