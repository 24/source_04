<?php

require_once('../../../php/tools/pb_tools.php');
require_once('../../../php/tools/pb_tools_utf8.php');

echo "\r   \r";
$g_data_dir = 'c:/Data/Exe/Print/';
Logg_SetFile($g_data_dir . 'dzsat/Log/Log.txt');
Logg_RAZ();


Test_01();

function Test_01() {
   global $g_data_dir;
   $url = 'http://www.dzsat.org/forum/';
   LoggLine("load url \"$url\"");
   $html = load_url($url);
   write_to_file($g_data_dir . 'dzsat/log_html/dzsat.org.forum.html', $html);
}


?>
