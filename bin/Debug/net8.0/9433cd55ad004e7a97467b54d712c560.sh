function list_child_processes () {
    local ppid=$1;
    local current_children=$(pgrep -P $ppid);
    local local_child;
    if [ $? -eq 0 ];
    then
        for current_child in $current_children
        do
          local_child=$current_child;
          list_child_processes $local_child;
          echo $local_child;
        done;
    else
      return 0;
    fi;
}

ps 35003;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 35003 > /dev/null;
done;

for child in $(list_child_processes 35006);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/reynald/My-Programs/DotnetCore/core8_nuxt_cassandra/bin/Debug/net8.0/9433cd55ad004e7a97467b54d712c560.sh;
