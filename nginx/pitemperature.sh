#!/bin/sh
### BEGIN INIT INFO
# Provides:          <pitemperature>
# Required-Start:    $local_fs $network $named $time $syslog
# Required-Stop:     $local_fs $network $named $time $syslog
# Default-Start:     2 3 4 5
# Default-Stop:      0 1 6
# Description:       Script to run asp.net 5 application in background
### END INIT INFO

# Author: Ivan Derevianko aka druss <drussilla7@gmail.com>
# Fixed for ASP.NET 5 beta 7 with DNVM/DNX by Michal A. Valasek - github.com/ridercz
# Added support for dnx-watch added with ASP.NET 5 beta 8, improved configurability,
# fixed 'nohup: ignoring input and redirecting stderr to stdout' message when starting - Anthony Stivers (github.com/tstivers1990)

# Set path variable so that dnx-watch can find dnx

WWW_USER=pi

# We can use either dnx or dnx-watch here
DNXRUNTIME=dnx
DNXFRAMEWORK=mono
DNXVERSION=1.0.0-rc1-update1
DNXCOMMAND=web

APPROOT=/home/$WWW_USER/PITemperature/src/PiTemperature

DNXPATHS=/home/$WWW_USER/.dnx/runtimes/dnx-$DNXFRAMEWORK.$DNXVERSION/bin:/home/$WWW_USER/.dnx/bin

PIDFILE=$APPROOT/pitemperature.pid
LOGFILE=$APPROOT/pitemperature.log

# fix issue with DNX exception in case of two env vars with the same name but different case
TMP_SAVE_runlevel_VAR=$runlevel
unset runlevel

start() {
  if [ -f $PIDFILE ] && kill -0 $(cat $PIDFILE); then
    echo 'Service already running' >&2
    return 1
  fi
  echo 'Starting service...' >&2
  su -c "export PATH=$DNXPATHS:\${PATH} && start-stop-daemon -SbmCv -x /usr/bin/nohup -p \"$PIDFILE\" -d \"$APPROOT\" -- \"$DNXRUNTIME\" $DNXCOMMAND > \"$LOGFILE\" 2>&1" $WWW_USER
  echo 'Service started' >&2
}

stop() {
  if [ ! -f "$PIDFILE" ] || ! kill -0 $(cat "$PIDFILE"); then
    echo 'Service not running' >&2
    return 1
  fi
  echo 'Stopping service...' >&2
  start-stop-daemon -K -p "$PIDFILE"
  rm -f "$PIDFILE"
  echo 'Service stopped' >&2
}

case "$1" in
  start)
    start
    ;;
  stop)
    stop
    ;;
  restart)
    stop
    start
    ;;
  *)
    echo "Usage: $0 {start|stop|restart}"
esac

export runlevel=$TMP_SAVE_runlevel_VAR

