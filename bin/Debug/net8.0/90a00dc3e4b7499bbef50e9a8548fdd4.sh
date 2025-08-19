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

ps 49286;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 49286 > /dev/null;
done;

for child in $(list_child_processes 49291);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/reynald/My-Programs/DotnetCore/core8_nuxt_cassandra/bin/Debug/net8.0/90a00dc3e4b7499bbef50e9a8548fdd4.sh;
