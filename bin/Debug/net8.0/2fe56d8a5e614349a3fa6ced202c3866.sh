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

ps 63374;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 63374 > /dev/null;
done;

for child in $(list_child_processes 63920);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/reynald/My-Programs/DotnetCore/core8_nuxt_cassandra/bin/Debug/net8.0/2fe56d8a5e614349a3fa6ced202c3866.sh;
