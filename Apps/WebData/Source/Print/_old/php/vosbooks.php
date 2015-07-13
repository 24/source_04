<?php

require_once('../../../php/tools/pb_tools.php');
require_once('../../../php/tools/pb_tools_utf8.php');

//Logg_SetFile();
Logg_RAZ();

// w3C XPath 1.0 http://xmlfr.org/w3c/TR/xpath/

$g_db_host = 'localhost';
$g_db_database = 'print_test';
$g_data_dir = '_files_\\vosbooks\\';

//load_vosbooks('print', 'print_url');
//load_vosbooks_print_page('print', 'print', $g_data_dir . 'html\\', 'paper');
//analyse_vosbooks('print', 'tmp_print_detail');
//load_vosbooks('tmp_print', 'tmp_print_url', 'vosbooks.com', 'magazine', 'http://www.vosbooks.com/liste-des-magazines', $g_data_dir . 'log_magazines_01.txt', $g_data_dir . 'liste-des-magazines_01.html');
//load_vosbooks('tmp_print', 'tmp_print_url', 'vosbooks.com', 'book', 'http://www.vosbooks.com/liste-des-livres', $g_data_dir . 'log_books_01.txt', $g_data_dir . 'liste-des-livres_01.html');
//load_vosbooks('tmp_print', 'tmp_print_url', 'vosbooks.com', 'paper', 'http://www.vosbooks.com/liste-des-journaux', $g_data_dir . 'log_papers_01.txt', $g_data_dir . 'liste-des-journaux_01.html');
load_vosbooks_print_page('tmp_print', 'tmp_print_url', $g_data_dir . 'html\\', 'paper');
//analyse_vosbooks('tmp_print', 'tmp_print_detail');

//control_analyse_vosbooks_02('_files_\vosbooks\control_magazines_01.txt');
//control_analyse_vosbooks_01();

//load_vosbooks_servers('servers', '_files_\\vosbooks\\servers_01.txt');
//select_vosbooks_print('print', 'print_url', 'tmp_select', '_files_\\vosbooks\\magazines_01.txt', 'vosbooks.com', 'magazine');



//test_get_values_01(utf8_encode(''));
//test_get_values_01(utf8_encode(''));
//test_get_values_01(utf8_encode(''));
//test_get_values_01(utf8_encode(''));
//test_get_values_01(utf8_encode(''));
//test_get_values_01(utf8_encode(''));
//test_get_values_01(utf8_encode(''));
//test_get_values_01(utf8_encode(''));
//test_get_values_01(utf8_encode(''));

//test_analyse_vosbooks_03();
//test_analyse_vosbooks_02();
//test_database_01();
//test_DOMDocument_01();
//test_DOMDocument_02('_files_\\vosbooks\\liste-des-magazines_01.html', '_files_\\vosbooks\\liste-des-magazines_03.html');
//test_DOMDocument_02('_files_\\vosbooks\\liste-des-magazines_01_03.html', '_files_\\vosbooks\\liste-des-magazines_01_03_03.html');
//test_DOMDocument_02('_files_\\vosbooks\\liste-des-magazines_01_04.html', '_files_\\vosbooks\\liste-des-magazines_01_04_03.html');
//test_DOMDocument_02('_files_\\vosbooks\\liste-des-magazines_01_05.html', '_files_\\vosbooks\\liste-des-magazines_01_05_03.html');
//test_DOMDocument_02('_files_\\vosbooks\\liste-des-magazines_01_06.html', '_files_\\vosbooks\\liste-des-magazines_01_06_03.html');
//test_check_utf8_01('_files_\\vosbooks\\log_magazines_analyse_01_39.txt', '_files_\\vosbooks\\log_magazines_analyse_01_39_2.txt');
//test_check_utf8_01('_files_\\vosbooks\\log_control_magazines_analyse_01_99.txt', '_files_\\vosbooks\\log_control_magazines_analyse_01_99_2.txt');
//test_check_utf8_01('_files_\\vosbooks\\liste-des-magazines_01.html', '_files_\\vosbooks\\liste-des-magazines_01_08.html');
//test_check_utf8_01('_files_\\vosbooks\\liste-des-magazines_01_06.html');
//test_load_download_page_01('http://www.vosbooks.com/6627-revues-magazines/hf-micro-hebdo-n%c2%b0671-du-24-fevrier-au-2-mars-2011-megaupload.html', '_files_\\vosbooks\\html\\');
//test_load_download_page_01('http://www.vosbooks.com/12839-revues-magazines/android-mobiles-tablettes-n%c2%b09-juin-juillet-aout-2011-megaupload.html', '_files_\\vosbooks\\html\\');
//test_load_download_page_01('http://www.vosbooks.com/9033-revues-magazines/a-vos-mac-tablettes-n%c2%b05-mars-avril-2011-megaupload.html', '_files_\\vosbooks\\html\\');
//test_load_download_page_01('http://www.vosbooks.com/4794-revues-magazines/automobile-4x4-suv-fevmars-2011-megaupload.html', '_files_\\vosbooks\\html\\');
//test_load_download_page_01('http://www.vosbooks.com/1394-revues-magazines/la-maison-ecologique-n%c2%b039-megaupload.html', '_files_\\vosbooks\\html\\');
//test_load_download_page_01('http://www.vosbooks.com/4764-revues-magazines/tennis-magazine-janvierfevrier-2011-megaupload.html', '_files_\\vosbooks\\html\\');
//test_load_download_page_01('http://www.vosbooks.com/14347-revues-magazines/le-journal-de-mickey-n%c2%b03093-du-28-septembre-au-4-octobre-2011-megaupload.html', '_files_\\vosbooks\\html\\');
//test_load_download_page_01('http://www.vosbooks.com/12521-revues-magazines/megaupload-magazine-n%c2%b07-megaupload.html', '_files_\\vosbooks\\html\\');
//test_load_download_page_01('http://www.vosbooks.com/12161-revues-magazines/megaupload-magazine-n%c2%b07-juin-juillet-aout-2011-megaupload.html', '_files_\\vosbooks\\html\\');
//test_load_download_page_01('http://www.vosbooks.com/8717-revues-magazines/public-n%c2%b0406-du-22-au-28-avril-2011-megaupload.html', '_files_\\vosbooks\\html\\');
//test_load_download_page_01('http://www.vosbooks.com/14885-revues-magazines/60-millions-de-consommateurs-hors-serie-n%c2%b0109-novembre-decembre-2011-megaupload.html', '_files_\\vosbooks\\html\\');
//test_load_download_page_01('http://www.vosbooks.com/14089-revues-magazines/action-auto-moto-n%c2%b0193-octobre-2011-megaupload.html', '_files_\\vosbooks\\html\\');
//test_load_download_page_01('http://www.vosbooks.com/3613-revues-magazines/click-and-load-n%c2%b014-janvier-fevrier-2011-megaupload.html', '_files_\\vosbooks\\html\\');
//test_load_download_page_01('http://www.vosbooks.com/4884-revues-magazines/01-informatique-business-et-technologie-du-27-janvier-au-02-fevrier-2011-megaupload.html', '_files_\\vosbooks\\html\\');
//test_load_download_page_01('http://www.vosbooks.com/4884-revues-magazines/01-informatique-business-et-technologie-du-27-janvier-au-02-fevrier-2011-megaupload.html', '_files_\\vosbooks\\html\\');
//test_load_download_page_01('http://www.vosbooks.com/2816-revues-magazines/net-janvier-2011-megaupload.html', '_files_\\vosbooks\\html\\');
//test_load_download_page_03('http://www.vosbooks.com/9033-revues-magazines/a-vos-mac-tablettes-n%c2%b05-mars-avril-2011-megaupload.html', '_files_\\vosbooks\\html\\');

// Anciens numéros, Date Journal, Date Magazin, Date Magazine, Date d’ouvrage, Date , Genre, Hébergeur, ISBN, Langue,
// Lien HotFile, Lien Hotfile, Lien Megaupload, Nombre de Pages, Nombre de magazines, Qualité, Release Date, Taille, Type, angue, download, image, info, title

function load_vosbooks($table_print, $table_print_url, $site, $type, $url, $log_file, $html_file) {
   //$table_print = 'print';
   //$table_print_url = 'print_url';
   $db = open_db();
   if (!$db->table_exist($table_print)) {
      LoggLine("create table $table_print");
	   $db->query(get_create_table_print_sql($table_print));
	}
   if (!$db->table_exist($table_print_url)) {
      LoggLine("create table $table_print_url");
	   $db->query(get_create_table_print_url_sql($table_print_url));
	}

   //$ch = curl_init();
   //curl_setopt($ch, CURLOPT_URL, $url);
   //curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
   //$html = curl_exec($ch);
   //curl_close($ch);

   //$url = 'http://www.vosbooks.com/liste-des-magazines';
   LoggLine("load url \"$url\"");
   $html = load_url($url);

   $check = check_utf8($html, $corrected_html, $result);
   if (!$check) {
      $html = $corrected_html;
      LoggLine("remove {$result['nb_bad_bytes']} bad utf8 char");
   }
   //'_files_\\vosbooks\\liste-des-magazines_01.html'
   write_to_file($html_file, $html);

   $magazines = get_magazines($html);

   //LoggLine(str_var_dump($magazines));
   //$log_file = '_files_\\vosbooks\\log_magazines_01.txt';
   delete_file($log_file);
   $nb = 0;
   $nb_insert = 0;
   //$site = 'vosbooks.com';
   //$type = 'magazine';
	//$db->query("delete from $table_print where site = '$site' and type = '$type'");
	//$nb_deleted_row = $db->affected_rows();
	//LoggLine("delete $table_print where site = '$site' and type = '$type' : $nb_deleted_row rows deleted");

   foreach ($magazines as $magazine) {
      if ($magazine['url_code'] == null) {
         LoggLine("url_code not found \"{$magazine['name']}\"  \"{$magazine['url_download_page']}\"");
         continue;
      }
      $print_id = get_print_id($db, $table_print, $magazine['url_code']);
      if ($print_id == null) {
         insert_print($db, $table_print, $site, $type, $magazine['name'], $magazine['url_code'], $magazine['url_download_page']);
         $nb_insert++;
         $op = 'insert';
      } else
         $op = 'none  ';
      append_to_file($log_file, "$op : \"{$magazine['name']}\"  \"{$magazine['url_code']}\"  \"{$magazine['url_download_page']}\"" . CRLF);
      $nb++;
   }
   LoggLine("$nb magazines, $nb_insert magazines inserted");
   $db->close();
}

function load_vosbooks_print_page($table_print, $table_print_url, $log_dir, $type) {
   //$table_print = 'print';
   //$table_print_url = 'print_url';
   $db = open_db();

   //$res = $db->query("select * from $table_print order by print_id");
   //$res = $db->query("select * from $table_print where type = 'book' order by print_id");
   $res = $db->query("select * from $table_print where type = '$type' order by print_id");
   //$res = $db->query("select * from $table_print where name like '%e-books%'");
   //$res = $db->query("select d.* from tmp_error e inner join $table_print d on d.url_code = e.url_code");
   //$res = $db->query("select d.* from $table_print d left join tmp_error e on e.url_code = d.url_code where e.url_code is null order by print_id");
   //$res = $db->query("select * from $table_print where title like '%Megaupload%' order by print_id");
   //$res = $db->query("select * from $table_print where title is null or title like ' %' order by print_id");
   //$res = $db->query("select * from $table_print where title like '% ' or title like ' %' order by print_id");
   //$res = $db->query("select * from $table_print where title is null order by print_id");
   $i = 1;
   while ($row = mysql_fetch_array($res, MYSQL_ASSOC)) {
      $url = $row['url_download_page'];
      $url_code = $row['url_code'];
      LoggLine("load url ($i) \"$url\"");
      $result = load_download_page($url, $url_code, $log_dir);
      //LoggLine(str_var_dump($result));
      update_print_info($db, $table_print, $table_print_url, $row['print_id'], $result);
      $i++;
   }

   $db->close();
}

function analyse_vosbooks($table_print, $table_print_detail) {
   $log_file = '_files_\vosbooks\log_magazines_analyse_01_99.txt';
   delete_file($log_file);

   //$table_print = 'print';
   //$table_print_detail = 'tmp_print_detail';
   $db = open_db();
   if (!$db->table_exist($table_print_detail)) {
      LoggLine("create table $table_print_detail");
	   $db->query(get_create_table_print_detail_sql($table_print_detail));
	}

   $db->query("delete from $table_print_detail");

   //$res = $db->query("select * from $table_print order by name");
   $res = $db->query("select * from $table_print order by print_id");
   while ($row = mysql_fetch_array($res, MYSQL_ASSOC)) {
      $name = $row['name'];
      $values = array();
      $values['print_id'] = $row['print_id'];
      $values['name'] = $name;
      $name2 = get_values($name, $values);

      $name2 = uft8_string_normalize($name2);
      //$name2 = strtolower($name2);
      $name2 = mb_strtolower($name2, "utf8");
      $values['print_paper'] = $name2;
      //insert_tmp_magazines($db, $values);
      insert_print_detail($db, $table_print_detail, $values);
      log_magazines($log_file, $values);
   }
   append_to_file($log_file, "************************************************************************************************************************" . CRLF);

   $sql = "update $table_print d inner join $table_print_detail t on t.print_id = d.print_id";
   $sql .= " set d.print_name = null, d.print_paper = t.print_paper, d.print_date = t.print_date, d.print_number = t.print_number, d.attrib1 = t.attrib1, d.attrib2 = t.attrib2, d.attrib3 = t.attrib3, d.attrib4 = t.attrib4";
   $db->query($sql);

   $db->close();
}

function control_analyse_vosbooks_02($file) {
   $log_file = '_files_\vosbooks\log_control_magazines_01_99.txt';
   delete_file($log_file);
   $data = read_file($file);
   $names = explode("\r\n", $data);
   foreach ($names as $name) {
      if ($name == "") continue;
      test_analyse_vosbooks_01($log_file, $name);
   }
   append_to_file($log_file, "************************************************************************************************************************" . CRLF);
}

function load_vosbooks_servers($table_servers, $file) {
   //$table_servers = 'servers';
   $db = open_db();
   if (!$db->table_exist($table_servers)) {
      LoggLine("create table $table_servers");
	   $db->query(get_create_table_servers_sql($table_servers));
	}

	$db->query("delete from $table_servers");
	$nb = $db->affected_rows();
   LoggLine("delete $nb rows in $table_servers");

   LoggLine("import file \"$file\"");
   $file = realpath($file);
   $file = str_replace('\\', '/', $file);

	$sql = "load data infile '$file' into table $table_servers character set utf8 fields terminated by '\\t' escaped by '' lines terminated by '\\r\\n' ( server_order, server )";
	$db->query($sql);
	$nb = $db->affected_rows();
   LoggLine("import $nb rows in $table_servers");

   $db->close();
}

function select_vosbooks_print($table_print, $table_print_url, $table_select, $file, $site, $type) {
   //$table_print = 'print';
   //$table_print_url = 'print_url';
   //$table_select = 'tmp_select';
   $db = open_db();
   if (!$db->table_exist($table_select)) {
      LoggLine("create table $table_select");
	   $db->query(get_create_table_select_sql($table_select));
	}
	$db->query("delete from $table_select");
	$nb = $db->affected_rows();
   LoggLine("delete $nb rows in $table_select");

   LoggLine("import file \"$file\"");
   $file = realpath($file);
   $file = str_replace('\\', '/', $file);
   //LoggLine("import file \"$file\"");

	$sql = "load data infile '$file' into table $table_select character set utf8 fields terminated by '\\t' escaped by '' lines terminated by '\\r\\n' ( name )";
	$sql .= " set site = " . sql_value($site, "string") . ", type = " . sql_value($type, "string");
	$db->query($sql);
	$nb = $db->affected_rows();
   LoggLine("import $nb rows in $table_select");

   $sql = "update $table_select s inner join print d on d.site = s.site and d.type = s.type and d.name = s.name set s.print_id = d.print_id";
	$db->query($sql);

   $db->close();
}

function test_analyse_vosbooks_03() {
   $log_file = '_files_\vosbooks\log_test_analyse_vosbooks_03.txt';
   delete_file($log_file);

   $table_print = 'print';
   $db = open_db();

   $res = $db->query("select * from $table_print where print_id = 1028 order by print_id");
   while ($row = mysql_fetch_array($res, MYSQL_ASSOC)) {
      $name = $row['name'];
      $values = array();
      $values['print_id'] = $row['print_id'];
      $values['name'] = $name;

      LoggLine("name    \"$name\"");
      $name2 = get_values($name, $values);

      LoggLine("name2 1 \"$name2\"");
      $check = check_utf8($name2, $corrected_string, $result);
      if (!$check) {
         LoggLine("check utf8 name2 1 \"$name2\" : " . str_var_dump($check));
         LoggLine(str_var_dump($result));
         break;
      }

      $name2 = uft8_string_normalize($name2);
      LoggLine("name2 2 \"$name2\"");
      $check = check_utf8($name2, $corrected_string, $result);
      if (!$check) {
         LoggLine("check utf8 name2 2 \"$name2\" : " . str_var_dump($check));
         LoggLine(str_var_dump($result));
         break;
      }

      //$name2 = strtolower($name2);
      $name2 = mb_strtolower($name2, "utf8");
      LoggLine("name2 3 \"$name2\"");
      $check = check_utf8($name2, $corrected_string, $result);
      if (!$check) {
         LoggLine("check utf8 name2 3 \"$name2\" : " . str_var_dump($check));
         LoggLine(str_var_dump($result));
         break;
      }

      $values['print_paper'] = $name2;
      log_magazines($log_file, $values);
   }

   append_to_file($log_file, "************************************************************************************************************************" . CRLF);
   $db->close();
}

function test_analyse_vosbooks_02() {
   $log_file = '_files_\vosbooks\log_control_01_99.txt';
   delete_file($log_file);

   //$name = utf8_encode('Auto Express – New Year Special 2011');
   $name = utf8_encode('Auto Express - New Year Special 2011');

   LoggLine("get_values : \"$name\"");
   $values = array();
   $values['name'] = $name;
   $name2 = get_values($name, $values);
   $name2 = uft8_string_normalize($name2);
   //$name2 = strtolower($name2);
   $name2 = mb_strtolower($name2, "utf8");
   $values['print_paper'] = $name2;
   log_magazines($log_file, $values);
}

function log_magazines($log_file, $values) {
   $log = "************************************************************************************************************************" . CRLF
      //.                                        "print_id          = {$values['print_id']}"                    . CRLF
      . (isset($values['print_id'])          ? "print_id          = {$values['print_id']}"              : '') . CRLF
      .                                        "name              = \"{$values['name']}\""                    . CRLF
      . (isset($values['name2'])             ? "name2             = \"{$values['name2']}\""             : '') . CRLF
      . (isset($values['print_name'])        ? "print_name        = \"{$values['print_name']}\""        : '') . CRLF
      . (isset($values['print_paper'])       ? "print_paper       = \"{$values['print_paper']}\""       : '') . CRLF
      . (isset($values['print_date'])        ? "print_date        = \"{$values['print_date']}\""        : '') . CRLF
      . (isset($values['print_number'])      ? "print_number      = \"{$values['print_number']}\""      : '') . CRLF
      . (isset($values['attrib1'])           ? "attrib1           = \"{$values['attrib1']}\""           : '') . CRLF
      . (isset($values['attrib2'])           ? "attrib2           = \"{$values['attrib2']}\""           : '') . CRLF
      . (isset($values['attrib3'])           ? "attrib3           = \"{$values['attrib3']}\""           : '') . CRLF
      . (isset($values['attrib4'])           ? "attrib4           = \"{$values['attrib4']}\""           : '') . CRLF
      . (isset($values['attrib5'])           ? "attrib5           = \"{$values['attrib5']}\"" . CRLF    : '')
      . (isset($values['attrib6'])           ? "attrib6           = \"{$values['attrib6']}\"" . CRLF    : '')
      . (isset($values['attrib7'])           ? "attrib7           = \"{$values['attrib7']}\"" . CRLF    : '')
      . (isset($values['attrib8'])           ? "attrib8           = \"{$values['attrib8']}\"" . CRLF    : '')
      . CRLF;
   append_to_file($log_file, $log);
   if (isset($values['attrib5']))
      LoggLine("error to much attrib : \"{$values['name']}\"");
}

function get_values($name, &$values) {
   //LoggLine("name 1 : \"$name\"");
   $name = get_attribs($name, $values, $attrib_number);
   //LoggLine("name 2 : \"$name\"");
   $name = get_number($name, $values);
   //LoggLine("name 3 : \"$name\"");
   //$name = get_date($name, $values);
   $name = get_date2($name, $values);
   //LoggLine("name 4 : \"$name\"");
   $name = get_attribs2($name, $values, $attrib_number);
   //LoggLine("name 5 : \"$name\"");
   $name = str_replace('@', ' ', $name);
   //LoggLine("name 6 : \"$name\"");
   //$name = string_remove_multiple_space($name);
   $name = uft8_string_remove_multiple_space($name);
   // à : C3-A0
   //$name = preg_replace('@\s{2,}@', ' ', $name);
   //LoggLine("name 7 : \"$name\"");
   $name = trim($name);
   //LoggLine("name 8 : \"$name\"");
   return $name;
}

function get_attribs($s, &$values, &$attrib_number) {
   $attrib_number = 1;

   $pattern = "@^\\s*\\[([^\\]]*)\\]@";
   $r = preg_match($pattern, $s, $matches, PREG_OFFSET_CAPTURE);
   //LoggLine("get_attribs 3 : result $r  pattern \"$pattern\"  string \"$s\"");
   if ($r == 1) {
      $values['attrib' . $attrib_number++] = trim($matches[1][0]);
      $p = $matches[0][1];
      $l = strlen($matches[0][0]);
      //$s = trim(substr($s, 0, $p), " \t\r\n,()[]-\xC2\xA0\xE2\x80\x93");
      $s = pb_trim(substr($s, $p + $l));
   }

   $pattern = '@\(([^\)^0-9]+)\)@';
   //$r = preg_match_all($pattern, $s, $matches, PREG_PATTERN_ORDER | PREG_OFFSET_CAPTURE);
   //$s2 = '';
   //$p = 0;
   //for ($i = 0; $i < $r; $i++) {
      //$p2 = $matches[0][$i][1];
      //$l = strlen($matches[0][$i][0]);
      //if ($s2 != '') $s2 .= ' @ ';
      //$s2 .= pb_trim(substr($s, $p, $p2 - $p));
      //$p = $p2 + $l;
      //$values['attrib' . $attrib_number++] = $matches[1][$i][0];
   //}
   //if ($s2 != '') $s2 .= ' @ ';
   //$s2 .= pb_trim(substr($s, $p, strlen($s) - $p));
   $s = match_all($s, $pattern, 1, $values2);
   foreach ($values2 as $value)
      $values['attrib' . $attrib_number++] = $value;

   // Action Auto Moto N°189 Juin 2011 Mu
   //$s = $s2;
   $attribs = 'mu';
   $pattern = "@\\s+($attribs)\\s*$@i";
   $r = preg_match($pattern, $s, $matches, PREG_OFFSET_CAPTURE);
   //LoggLine("get_attribs 3 : result $r  pattern \"$pattern\"  string \"$s\"");
   if ($r == 1) {
      $values['attrib' . $attrib_number++] = trim($matches[1][0]);
      $p = $matches[0][1];
      //$s = trim(substr($s, 0, $p), " \t\r\n,()[]-\xC2\xA0\xE2\x80\x93");
      $s = pb_trim(substr($s, 0, $p));
   }

   // Combat Aircraft Monthly Decembre 2010
   // Cosmopolitan France Fevrier 2011
   // Cosmopolitan UK Fevrier 2011
   // Decormag Québec Octobre 2011 
   $attribs = utf8_encode('uk|france|québec|quebec|australia|best of|monthly');
   $pattern = "@\\s+($attribs)(\\s+|$)@i";
   //$r = preg_match_all($pattern, $s, $matches, PREG_PATTERN_ORDER | PREG_OFFSET_CAPTURE);
   //$s2 = '';
   //$p = 0;
   //for ($i = 0; $i < $r; $i++) {
      //$p2 = $matches[0][$i][1];
      //$l = strlen($matches[0][$i][0]);
      //if ($s2 != '') $s2 .= ' @ ';
      //$s2 .= pb_trim(substr($s, $p, $p2 - $p));
      //$p = $p2 + $l;
      //$values['attrib' . $attrib_number++] = $matches[1][$i][0];
   //}
   //if ($s2 != '') $s2 .= ' @ ';
   //$s2 .= pb_trim(substr($s, $p, strlen($s) - $p));

   $s = match_all($s, $pattern, 1, $values2);
   foreach ($values2 as $value)
      $values['attrib' . $attrib_number++] = $value;

   // Click & Load Collection 2010

   //pack
   //$s = $s2;
   $pattern = "@\\s*(pack)\\s+@i";
   $r = preg_match($pattern, $s, $matches, PREG_OFFSET_CAPTURE);
   //LoggLine("get_attribs 3 : result $r  pattern \"$pattern\"  string \"$s\"");
   if ($r == 1) {
      $values['attrib' . $attrib_number++] = trim($matches[1][0]);
      $p = $matches[0][1] + strlen($matches[0][0]);
      //$s = trim(substr($s, 0, $p), " \t\r\n,()[]-\xC2\xA0\xE2\x80\x93");
      $s = pb_trim(substr($s, $p));
   }

   return trim($s);
}

function get_attribs2($s, &$values, &$attrib_number) {

   // Auto Express – New Year Special 2011

   $pattern = utf8_encode('@(hors[\s-]+série|hors[\s-]+serie|hs)(.*)$@i');
   $r = preg_match($pattern, $s, $matches, PREG_OFFSET_CAPTURE);
   if ($r == 1) {
      $values['attrib' . $attrib_number++] = utf8_encode('hors-série');
      $attrib = pb_trim2($matches[2][0]);
      if ($attrib != '')
         $values['attrib' . $attrib_number++] = $attrib;
      $p = $matches[0][1];
      $s = pb_trim(substr($s, 0, $p));
   }

   //$pattern = utf8_encode('#\s[\+\-/]\s*([^@*)$#i');
   $pattern = utf8_encode('#\s[\+\-/]\s*([^@]*)#i');
   //$pattern = utf8_encode('#(\s\-|[\+/])\s*([^@]*)#i');
   $r = preg_match($pattern, $s, $matches, PREG_OFFSET_CAPTURE);
   if ($r == 1) {
      $attrib = pb_trim2($matches[1][0]);
      if ($attrib != '')
         $values['attrib' . $attrib_number++] = $attrib;
      //$p = $matches[0][1];
      //$s = pb_trim(substr($s, 0, $p));

      $p = $matches[0][1];
      $p2 = $p + strlen($matches[0][0]);
      $l = strlen($s);
      $s = pb_trim(pb_trim(substr($s, 0, $p)) . ' @ ' . pb_trim(substr($s, $p2, $l - $p2)));
   }

   $pattern = utf8_encode('#@([^@]*)#i');
   $s = match_all($s, $pattern, 1, $values2);
   foreach ($values2 as $value) {
      $value = pb_trim($value);
      if ($value != '')
         $values['attrib' . $attrib_number++] = $value;
   }


   return $s;
}

function get_date($s, &$values) {
   // du 17 au 23 Fevrier 2011
   // Du 27 Janvier au 02 Février 2011 
   // Semaine du 17/23 décembre 2010

   //$r = preg_match("@\\s+(semaine\\s+du|du)\\s+(.*)$@i", $s, $matches, PREG_OFFSET_CAPTURE);
   $r = preg_match("@\\s+((semaine\\s+du|du)\\s+.*)$@i", $s, $matches, PREG_OFFSET_CAPTURE);
   if ($r == 1) {
      $values['print_date'] = $matches[1][0];
      return trim(substr($s, 0, $matches[0][1]));
   }

   // 20 Janvier 2011
   // 3eme Trimestre 2011
   // Juillet 2011
   // Été 2011
   // Novembre Décembre 2011
   // Janv à Fev 2011
   // Janvier/Février 2011
   // Janvier-Fevrier-Mars 2011

   //*8/14 janvier 2011                                                           -  314 - Closer N°291 - 8/14 janvier 2011
   //*February (2011)                                                             -  358 - Computer Shopper  February (2011)
   //*Sep/Octobre 2010                                                            -  423 - Creanum N°144 Sep/Octobre 2010
   //*(Mars/avril/mai 2011)                                                       - 2067 - Web Pocket n°8 (Mars/avril/mai 2011)
   // 23 December to 05 January 2010                                              -  353 - Computer Active issue 335 23 December to 05 January 2010 (UK)
   // Vendredi 24 Decembre / Jeudi 13 Janvier 2011                                -  168 - Bakchich Hebdo N 52 Vendredi 24 Decembre / Jeudi 13 Janvier 2011
   // January 2011 Avril 2011                                                     -  685 - Hawaii Drive Guides - Oahu Maps January 2011 Avril 2011
   // Nov-Decembe 2010/Janvier 2011                                               -  424 - Creanum N°145 Nov-Decembe 2010/Janvier 2011
   // Dec 2010/Janvier 2011                                                       - 1376 - Moins cher Dec 2010/Janvier 2011
   // Dec 2010/Janvier 2011                                                       - 1920 - Terre Information Magazine N°220 Dec 2010/Janvier 2011
   // December 16th 2010                                                          -  163 - Autosport Magazine, December 16th 2010
   // 2011.01.03                                                                  -  453 - Daily Mail 2011.01.03
   // (1998)                                                                      - 2154 - [La guerre des serpents-4]Les fragments d’une couronne brisee(1998)
   // [Janv à Fev 2011]                                                           -   49 - Action Auto Moto N 185 [Janv à Fev 2011]

   $weekday = utf8_encode("(lundi|monday|mardi|tuesday|mercredi|wednesday|jeudi|thursday|vendredi|friday|samedi|saterday|dimanche|sunday)\\s+");
   //$month = utf8_encode("(trimestre|Été|été|ete|automne|hiver|winter|printemps|janv|janvier|january|fev|fév|février|fevrier|february|mars|march|avril|april|mai|may|juin|june|juillet|july|août|aout|august|septembre|september|sep|octobre|october|nov|novembre|november|décembre|decembre|december)[aà\\s-/]+");
   $month = utf8_encode("(au|trimestre|Été|été|ete|automne|hiver|winter|printemps|jan|janv|janvier|january|fev|fév|février|fevrier|fevrie|february|mars|march|avril|april|mai|may|juin|june|juillet|july|août|aout|august|septembre|september|sep|octobre|october|nov|novembre|november|dec|déc|décembre|decembre|december)[à\\s-/]+");
   //$day = utf8_encode("(1er|2eme|2ème|3eme|3ème|4eme|4ème|[0-9]+)\\s+");
   $day = utf8_encode("((1er|2eme|2ème|3eme|3ème|4eme|4ème|[0-9]+)[\\s/-]+)+");
   //$year = "[0-9]{3,4}";
   //$year = "\\(?[0-9]{3,4}\\)?";
   $year = "\\(?[0-9]{3,4}";
   //$begin_sep = "((-|\xE2\x80\x93|\\s)*|^)[\\[\\(]?";  // E2-80-93 = tiret
   $begin_sep = "((-|\xE2\x80\x93|\\s|,)+|^)[\\[\\(]?";  // E2-80-93 = tiret
   $end_sep = "(\\s*[-/\\]\\)]?)";
   //$end_sep = "((\\s*[-/\\]\\)]?)|$)";
   //$end_sep = "(\\s+[-/\\]\\)]?|$)";
   //$pattern = "@$begin_sep($weekday)?(($day)?($month)+$year)$end_sep@i";
   $pattern = "@$begin_sep($weekday)?($day)?($month)+($year)?$end_sep@i";
   //$pattern = "@$begin_sep(($weekday)?(($day)?($month)+($year)?)$end_sep)+@i";
   $date = '';
   while (($r = preg_match($pattern, $s, $matches, PREG_OFFSET_CAPTURE)) == 1) {
   //$r = preg_match_all($pattern, $s, $matches, PREG_PATTERN_ORDER | PREG_OFFSET_CAPTURE);
   //LoggLine(str_var_dump($matches));
   //if ($r == 1) {
   //$s2 = '';
   //$p = 0;
   //$date = '';
   //for ($i = 0; $i < $r; $i++) {
      //LoggLine(str_var_dump($matches));
      // char espace (nbsp) code 160  C2-A0, char tiret code ???  E2-80-93
      //$d = $matches[0][$i][0];
      $d = $matches[0][0];
      if ($date != '') $date .= ' ';
      //$date .= trim($d, " \t\r\n,()[]-\xC2\xA0\xE2\x80\x93");
      $date .= pb_trim($d);
      $p = $matches[0][1];
      $p2 = $p + strlen($d);
      $l = strlen($s);
      $s = trim(trim(substr($s, 0, $p)) . ' @ ' . trim(substr($s, $p2, $l - $p2)));
      //$p2 = $matches[0][$i][1];
      //if ($s2 != '') $s2 .= ' ';
      //$s2 .= trim(substr($s, $p, $p2 - $p), " \t\r\n,()[]-\xC2\xA0\xE2\x80\x93");
      //$p = $p2 + strlen($d);
   }
   //if ($s2 != '') $s2 .= ' ';
   //$s2 .= trim(substr($s, $p, strlen($s) - $p), " \t\r\n,()[]-\xC2\xA0\xE2\x80\x93");

   if ($date != '')
      $values['print_date'] = $date;


   //$r = preg_match("@\\s+(-|\xE2\x80\x93)\\s+(.*)$@i", $s, $matches, PREG_OFFSET_CAPTURE);
   //if ($r == 1) {
      //$values['print_date'] = $matches[2][0];
      //return trim(substr($s, 0, $matches[0][1]));
   //}
   //return $s;
   //return trim($s2, " \t\r\n,()[]-\xC2\xA0\xE2\x80\x93");
   //return trim($s, " \t\r\n,()[]-\xC2\xA0\xE2\x80\x93");
   return pb_trim($s);
}

function get_date2($s, &$values) {
   $weekday = utf8_encode("lundi|monday|mardi|tuesday|mercredi|wednesday|jeudi|thursday|vendredi|friday|samedi|saterday|dimanche|sunday");
   $month = utf8_encode("trimestre|eté|Été|été|ete|automne|hiver|winter|printemps|janvier|january|janv|jan|février|fevrier|fevrie|fev|fév|february|mars|march|avril|april|mai|may|juin|june|juillet|july|août|aout|august|septembre|september|sep|sept|octobre|october|novembre|november|nov|décembre|decembre|december|decembe|dec|déc");
   $num1 = utf8_encode("1er|1st|[2-9]+eme|[2-9]+ème|[0-9]+th");
   //$num2 = "[0-9]+";
   $num2 = "[0-9]|[0-3][0-9]|19[0-9]{2}|20[0-9]{1,2}";
   $all = "$weekday|$month|$num1|$num2";
   //$begin_sep = "((-|\xE2\x80\x93|\\s|,|du\\s)+|^)[\\[\\(]?";  // E2-80-93 = tiret
   //$begin_sep = "((-|\xE2\x80\x93|\\s|,|du\\s|semaine\\s+du\\s)+)[\\[\\(]?";  // E2-80-93 = tiret
   $begin_sep = "((du\\s|\\sle\\s|semaine\\s+du\\s|-|\xE2\x80\x93|\\s|,|\\()+)[\\[\\(]?";  // E2-80-93 = tiret
   //$end_sep = "(\\s*[-/\\]\\)]?)";
   $end_sep = "([\\s-/\\]\\)]+)|$";
   $sep = "(\xE2\x80\x93)" . utf8_encode("|au\\s|to\\s|a\\s|à\\s|et\\s|de\\s|[\\s,-/\\[\\]\\(\\)]");
   //$pattern = "@$begin_sep(($weekday)|($month)|($num1)|($num2)|($sep))+@i";
   //$pattern = "@$begin_sep($all)(($sep)+($all))*@i";
   //$pattern = "@$begin_sep($all)(($sep)+($all))*($end_sep)@i";
   $pattern = "@$begin_sep($all)(($sep)+($all)|($num2)($weekday|$month))*($end_sep)@i";

   //$begin_sep = "(-|\\s|,|du\\s)+";  // E2-80-93 = tiret
   //$all = utf8_encode("février|fevrier|fevrie|fev|fév|february|[0-9]+");
   //$sep = utf8_encode("[\\s,-/\\[\\]\\(\\)]|(\xE2\x80\x93)|au\\s|a\\s|à\\s");
   //$pattern = "@$begin_sep($all)(($sep)+($all))*@i";


   //$r = preg_match($pattern, $s, $matches, PREG_OFFSET_CAPTURE);
   //LoggLine("get_date2 :");
   //LoggLine(" pattern : \"$pattern\"");
   //LoggLine(" result  : $r");
   //LoggLine(str_var_dump($matches));

   //if ($r == 1) {
      //$d = $matches[0][0];
      //$date = trim($d, " \t\r\n,()[]-\xC2\xA0\xE2\x80\x93");
      //if ($date != '')
         //$values['print_date'] = $date;
      //$p = $matches[0][1];
      //$p2 = $p + strlen($d);
      //$l = strlen($s);
      //$s = trim(trim(substr($s, 0, $p)) . ' @ ' . trim(substr($s, $p2, $l - $p2)));
      //$s = trim($s, " \t\r\n,()[]-\xC2\xA0\xE2\x80\x93");
   //}


   $r = preg_match_all($pattern, $s, $matches, PREG_PATTERN_ORDER | PREG_OFFSET_CAPTURE);
   //LoggLine("get_date2 :");
   //LoggLine(" result  : $r");
   //LoggLine(str_var_dump($matches));
   if ($r >= 1) {
      $sel = 0;
      $d = $matches[0][0][0];
      $l = strlen($d);
      for ($i = 1; $i < $r; $i++) {
         $d2 = $matches[0][$i][0];
         $l2 = strlen($d2);
         if ($l2 > $l) {
            $sel = $i;
            $d = $d2;
            $l = $l2;
         }
      }
      $date = pb_trim($d);
      //if ($date != '' && (!is_numeric($date) || strlen($date) >= 4)) {
      //$r = preg_match('@[a-z]{3}@i', $date);
      // propriété des caractères unicodes, utiliser l'option u (PCRE8) traite une chaine utf8
      // \p{L} ou \pL : lettre
      // \p{N} ou \pN : nombre
      //$r = preg_match('@[a-z]{3}@i', $date);
      $r = preg_match('@\pL{3}|\pN{4}[\s\./]\pN{2}[\s\./]\pN{2}@iu', $date);
      //if ($date != '' && !is_numeric($date)) {
      if ($r == 1) {
         //if ($date != '')
         $values['print_date'] = $date;
         $p = $matches[0][$sel][1];
         $p2 = $p + $l;
         $s = trim(trim(substr($s, 0, $p)) . ' @ ' . trim(substr($s, $p2, strlen($s) - $p2)));
         $s = pb_trim($s);
      }
   }

   return $s;
}

function get_number($s, &$values) {
   //NÂ°
   // Computer Active - issue 336 Du 06 Janvier au 16 Janvier 2011 
   // Courrier International N°1082-83-84 du 28 juillet au 17 aout 2011
   //$pattern = utf8_encode('@[\\s{\\-/]+((n\s*(°)?)|issue\s)\s*(([0-9]+)((-|_|\s*à\s*)([0-9]+))*)@i');
   //$pattern = utf8_encode('@[\\s{\\-/]+(issue\s#?\s*|le\s+n|no\s*\.\s*|#\s*|(n\s*(°)?))\s*(([0-9]+)((-|_|\s*à\s*)([0-9]+))*)@i');
   $pattern = utf8_encode('@[\\s{\\-/]+(issue\s#?\s*|le\s+n|no\s*\.\s*|#\s*|(n\s*(°)?))\s*(([0-9]+)((-\s*|_\s*|\.\s*|\s*à\s*)([0-9]+))*)@i');
   //$r = preg_match($pattern, $s, $matches, PREG_OFFSET_CAPTURE);
   ////LoggLine("get_number : \"$s\"");
   ////LoggLine(str_var_dump($matches));
   //if ($r == 1) {
      //$values['print_number'] = $matches[4][0];
      //$p = $matches[0][1];
      //$p2 = $p + strlen($matches[0][0]);
      //$l = strlen($s);
      //return pb_trim(trim(substr($s, 0, $p)) . ' @ ' . trim(substr($s, $p2, $l - $p2)));
   //}

   $s = match_all($s, $pattern, 4, $values2);
   if (count($values2) > 0)
      $values['print_number'] = implode(' ', $values2);

   $r = preg_match(utf8_encode('@n°\s*(([0-9]+)((-|\s*à\s*)([0-9]+))*)@i'), $s, $matches, PREG_OFFSET_CAPTURE);
   //LoggLine("get_number : \"$s\"");
   //LoggLine(str_var_dump($matches));
   if ($r == 1) {
      //$values['print_number'] = intval($matches[4][0]);
      $values['print_number'] = $matches[1][0];
      $p = $matches[0][1];
      $p2 = $p + strlen($matches[0][0]);
      $l = strlen($s);
      //return trim(trim(substr($s, 0, $p)) . ' @ ' . trim(substr($s, $p2, $l - $p2)), " \t\r\n,()[]-\xC2\xA0\xE2\x80\x93");
      return pb_trim(trim(substr($s, 0, $p)) . ' @ ' . trim(substr($s, $p2, $l - $p2)));
   }

   return $s;
}

function match_all($s, $pattern, $n, &$values) {
   $values = array();
   $r = preg_match_all($pattern, $s, $matches, PREG_PATTERN_ORDER | PREG_OFFSET_CAPTURE);
   //LoggLine("match_all : \"$s\"");
   //LoggLine(str_var_dump($matches));
   $s2 = '';
   $p = 0;
   for ($i = 0; $i < $r; $i++) {
      $p2 = $matches[0][$i][1];
      $l = strlen($matches[0][$i][0]);
      if ($s2 != '') $s2 .= ' @ ';
      $s2 .= pb_trim(substr($s, $p, $p2 - $p));
      $p = $p2 + $l;
      $values[] = $matches[$n][$i][0];
   }
   if ($s2 != '') $s2 .= ' @ ';
   $s2 .= pb_trim(substr($s, $p, strlen($s) - $p));
   return $s2;
}

function pb_trim($s) {
   return trim($s, " \t\r\n,()[]{}/-+&\xC2\xA0\xE2\x80\x93");
}

function pb_trim2($s) {
   return trim($s, " \t\r\n,()[]{}/-+\xC2\xA0\xE2\x80\x93@");
}

function test_load_download_page_01($url, $dir) {
   LoggLine("load url \"$url\"");
   $result = load_download_page($url, null, $dir);
   LoggLine(str_var_dump($result));
}

function test_load_download_page_03($url, $dir) {
   LoggLine("load url \"$url\"");
   //$result = load_download_page($url, null, $dir);
   $url_code = get_url_code($url);
   $file = $dir . $url_code . '.html';
   $html = load_url($url);
   if (!check_utf8($html, $corrected_html, $result)) {
      $html = $corrected_html;
      LoggLine("remove {$result['nb_bad_bytes']} bad utf8 char");
      LoggLine(str_var_dump($result));
   }
   write_to_file($file, $html);

   $dom = get_dom($html);
   $html2 = $dom->saveHTML();
   write_to_file('dom.html', $html2);
   if (!check_utf8($html2, $corrected_html2, $result)) {
      $html2 = $corrected_html2;
      LoggLine("remove {$result['nb_bad_bytes']} bad utf8 char");
      LoggLine(str_var_dump($result));
   }

   $xdom = new DOMXPath($dom);

   //<div class="entry">
   $nodes = $xdom->query("//div[@class='entry']");
   if ($nodes->length === 0) {
      LoggLine('<div class="entry"> not found');
      return null;
   }
   if ($nodes->length > 1) {
      LoggLine('to many <div class="entry">');
      return null;
   }
   $div = $nodes->item(0);

   $nodes = $xdom->query("descendant::text()", $div);
   foreach ($nodes as $node) {
      print_node($node);
   }

   //LoggLine(str_var_dump($result));
}

function load_download_page($url, $url_code, $dir) {
   //$url = 'http://www.vosbooks.com/6627-revues-magazines/hf-micro-hebdo-n%c2%b0671-du-24-fevrier-au-2-mars-2011-megaupload.html';
   //LoggLine("load url \"$url\"");

   if ($url_code == null)
      $url_code = get_url_code($url);
   //LoggLine(str_var_dump($matches));
   $file = $dir . $url_code . '.html';
   $html = load_url($url);
   if (!check_utf8($html, $corrected_html, $result)) {
      $html = $corrected_html;
      LoggLine("remove {$result['nb_bad_bytes']} bad utf8 char");
   }
   //LoggLine("export html to file \"$file\"");
   write_to_file($file, $html);
   //return get_download_page_info($html);
   return get_download_page_info($html);
}

function get_download_page_info($html) {
   $xdom = get_dom_xpath($html);

   $result = array();

   //<div class="entry">
   $nodes = $xdom->query("//div[@class='entry']");
   if ($nodes->length === 0) {
      LoggLine('<div class="entry"> not found');
      return null;
   }
   if ($nodes->length > 1) {
      LoggLine('to many <div class="entry">');
      return null;
   }
   $div = $nodes->item(0);

   //$nodes = $xdom->query("(.//img)[position()=1]/@src", $div);
   $nodes = $xdom->query("(.//img)[position()=1]", $div);
   if ($nodes->length == 1) {
      //$result['image_url'] = $nodes->item(0)->value;
      $attributes = $nodes->item(0)->attributes;
      $src = $attributes->getNamedItem('src');
      $result['image_url'] = $src->value;
      $title = $attributes->getNamedItem('title');
      $title = $title->value;
      if (stripos($title, 'Megaupload') === false)
         $result['title'] = trim($title, " \t\r\n\xC2\xA0,");
      //LoggLine("image : $img");
   }

   $nodes = $xdom->query("descendant::text()", $div);
   //$i = 0;
   $title = true;
   $info = false;
   $var_name = null;
   $last_title = null;
   foreach ($nodes as $node) {
      // attention &nbsp; a pour code 160 (0xA0) mais en utf8 le code est 0xC2 0xA0 (C2 A0 : 110[00010] 10[100000] =>   000 1010 0000 = 0A0)
      // donc il faut utiliser trim($value, "\xC2\xA0");
      $s = trim($node->nodeValue, " \t\r\n\xC2\xA0,");
      //LoggLine("text no $i : \"$s\"");
      if ($s != '') {
         //if ($i == 1) {
         $r = preg_match('@^([^:]+)(:\s*)+(.*)$@', $s, $matches);
         if ($r === 0 && $title) {
            // Telecharger, Megaupload
            if (strcasecmp(substr($s, 0, 11), 'Telecharger') != 0 && stripos($s, 'Megaupload') === false) {
               //$result['title2'] = $s;
               if (!isset($result['title']))
                  $result['title'] = $s;
               else if ($s !== $result['title'])
                  $result['info'][] = $s;
               $title = false;
               $info = true;
            } else
               $last_title = $s;
         } else {
            $title = false;
            //$r = preg_match('@^([^:^\s]+)\s*:$@', $s, $matches);
            //$r = preg_match('@^([^:]+)(:\s*)+(.*)$@', $s, $matches);
            if ($r == 1) {
               //$var_name = trim($matches[1]);
               $var_name = trim($matches[1], " \t\r\n\xC2\xA0,");
               $var_name = get_page_info_var_name($var_name);
               $s = trim($matches[3], " \t\r\n\xC2\xA0,");
               //$title = false;
               $info = false;
            }
            //else if ($var_name != null) {
            if ($s != '') {
               if ($var_name != null) {
                  $result[$var_name] = $s;
                  $var_name = null;
               } else if ($info) {
                  $result['info'][] = $s;
               }
            }
         }
         //$i++;
      }
   }

   if (!isset($result['title']) && $last_title != null) {
      $result['title'] = trim(str_ireplace(array('Telecharger', 'Megaupload'), '', $last_title), " \t\r\n\xC2\xA0,");
   }



   $nodes = $xdom->query("descendant::a/@href", $div);
   //if ($nodes->length > 1) {
      //$result['download_url'] = $nodes->item(0)->value;
   //}
   $print_url = array();
   $result['print_url'] = array();
   foreach ($nodes as $node) {
      // http://www.filesonic.com/file/52391657/2011-01-p2p.rar
      $url = $node->value;
      $server = null;
      $r = preg_match('@://(www\.)?([^/]+)/@', $url, $matches);
      if ($r != 1)
         continue;
      $server = $matches[2];
      if ($server == 'vosbooks.com')
         continue;
      if (array_key_exists($url, $print_url))
         continue;
      $print_url[$url] = $server;
      $result['print_url'][] = array('url' => $url, 'server' => $server);
   }


   return $result;
}

function get_page_info_var_name($var_name) {
   switch (strtolower(utf8_decode($var_name))) {
      case 'anciens numéros':
         return 'old_issue';
      case 'date journal':
      case 'date magazin':
      case 'date magazine':
      case 'date d’ouvrage':
      case 'date':
         return 'date';
      case 'genre':
         return 'kind';
      case 'hébergeur':
         return 'download_server';
      case 'isbn':
         return 'isbn';
      case 'langue':
      case 'angue':
         return 'language';
      case 'lien hotfile':
      case 'lien hotfile':
      case 'lien megaupload':
         return 'print_url';
      case 'nombre de pages':
         return 'page_nb';
      case 'nombre de magazines':
         return 'magazine_nb';
      case 'qualité':
         return 'quality';
      case 'release date':
         return 'release_date';
      case 'taille':
         return 'size';
      case 'type':
         return 'magazine_type';
      default:
         return $var_name;
   }
}

// info                                                                                 => string(810)
// old_issue       : Anciens numéros                                                    => string(2)
// date            : Date Journal, Date Magazin, Date Magazine, Date d’ouvrage, Date    => string(44)
// kind            : Genre                                                              => string(24)
// download_server : Hébergeur                                                          => string(23)
// isbn            : ISBN                                                               => string(10)
// language        : Langue, angue                                                      => string(9)
// print_url    : Lien HotFile, Lien Hotfile, Lien Megaupload                        => string(76)
// page_nb         : Nombre de Pages                                                    => string(9)
// magazine_nb     : Nombre de magazines                                                => string(2)
// quality         : Qualité                                                            => string(3)
// release_date    : Release Date                                                       => string(18)
// size            : Taille                                                             => string(51)
// type            : Type                                                               => string(3)
// print_url                                                                         => string(118)
// image_url                                                                            => string(377)
// title                                                                                => string(576)
function get_create_table_print_sql($table_print) {
   //$table_print = 'print';
   return "
      create table $table_print (
         print_id           int unsigned  not null auto_increment,
         site               varchar(50)   not null,
         type               varchar(50)   not null,
         name               varchar(200)  not null,
         ts                 datetime      null,
         downloaded         bool          null,
         downloaded_ts      datetime      null,
         url_code           varchar(100)  not null,
         url_download_page  varchar(200)  not null,
         print_name         varchar(100)  null,
         print_paper        varchar(200)  null,
         print_date         varchar(100)  null,
         print_number       varchar(100)  null,
         attrib1            varchar(50)   null,
         attrib2            varchar(50)   null,
         attrib3            varchar(50)   null,
         attrib4            varchar(50)   null,
         title              varchar(200)  null,
         download_server    varchar(50)   null,
         image_url          varchar(1000) null,
         date               varchar(100)  null,
         release_date       varchar(100)  null,
         kind               varchar(50)   null,
         language           varchar(50)   null,
         size               varchar(100)  null,
         magazine_type      varchar(20)   null,
         page_nb            varchar(20)   null,
         magazine_nb        varchar(20)   null,
         info               text          null,
         old_issue          varchar(20)   null,
         isbn               varchar(20)   null,
         quality            varchar(20)   null,
         primary key print_id (print_id),
         key name (site, type, name),
         unique key url_code (url_code)
      ) engine=MyISAM default charset=utf8";
}

function get_create_table_print_url_sql($table_print_url) {
   return "
      create table $table_print_url (
         print_id           int unsigned  not null,
         server             varchar(50)   not null,
         url                varchar(300)  not null,
         key print_id (print_id)
      ) engine=MyISAM default charset=utf8";
}

function get_create_table_print_paper_sql($table_print_paper) {
   //$table_print_paper = 'print_paper';
   return "
      create table $table_print_paper (
         print_paper_id     int unsigned  not null auto_increment,
         print_paper        varchar(200)  not null,
         print_category     varchar(100)  null,
         primary key print_paper_id (print_paper_id),
         unique key print_paper (print_paper),
         key print_category (print_category)
      ) engine=MyISAM default charset=utf8";
}

function get_create_table_select_sql($table_select) {
   return "
      create table $table_select (
         print_id        int unsigned  null,
         site               varchar(50)   not null,
         type               varchar(50)   not null,
         name               varchar(200)  not null,
         server             varchar(50)   null,
         url                varchar(300)  null,
         key name (site, type, name),
         key print_id (print_id)
      ) engine=MyISAM default charset=utf8";
}

function get_create_table_print_detail_sql($table_print_detail) {
   return "
      create table $table_print_detail (
         print_id           int unsigned  not null,
         name               varchar(200)  not null,
         name2              varchar(200)  null,
         print_name         varchar(100)  null,
         print_paper        varchar(200)  null,
         print_date         varchar(100)  null,
         print_number       varchar(100)  null,
         attrib1            varchar(50)   null,
         attrib2            varchar(50)   null,
         attrib3            varchar(50)   null,
         attrib4            varchar(50)   null,
         primary key print_id (print_id),
         key name (name)
      ) engine=MyISAM default charset=utf8";
}

function get_create_table_servers_sql($table_servers) {
   return "
      create table $table_servers (
         server_id          int unsigned  not null auto_increment,
         server             varchar(50)   not null,
         server_order       int           not null,
         primary key server_id (server_id),
         unique key server (server)
      ) engine=MyISAM default charset=utf8";
}

//function insert_tmp_magazines($db, $values) {
function insert_print_detail($db, $table_print_detail, $values) {
   //$table_tmp_print = 'tmp_print';
   $sql = "insert into $table_print_detail ( print_id, name, name2, print_name, print_paper, print_date, print_number, attrib1, attrib2, attrib3, attrib4 ) values ( "
      . sql_value($values['print_id'],     "num"   ) . ', '
      . sql_value($values['name'],            "string") . ', '
      . (isset($values['name2'])             ? sql_value($values['name2'],             "string") : 'null') . ', '
      . (isset($values['print_name'])        ? sql_value($values['print_name'],        "string") : 'null') . ', '
      . (isset($values['print_paper'])       ? sql_value($values['print_paper'],       "string") : 'null') . ', '
      . (isset($values['print_date'])        ? sql_value($values['print_date'],        "string") : 'null') . ', '
      . (isset($values['print_number'])      ? sql_value($values['print_number'],      "string") : 'null') . ', '
      . (isset($values['attrib1'])           ? sql_value($values['attrib1'],           "string") : 'null') . ', '
      . (isset($values['attrib2'])           ? sql_value($values['attrib2'],           "string") : 'null') . ', '
      . (isset($values['attrib3'])           ? sql_value($values['attrib3'],           "string") : 'null') . ', '
      . (isset($values['attrib4'])           ? sql_value($values['attrib4'],           "string") : 'null') . ' )';
   $db->query($sql);
}

function update_print_info($db, $table_print, $table_print_url, $print_id, $result) {
   //$table_print = 'print';
   //$table_print_url = 'print_url';
   $sql = "update $table_print set ";

   if (isset($result['title']))           $sql .= 'title           = ' . sql_value($result['title'],           "string") . ', '; else $sql .= 'title           = null, ';
   if (isset($result['download_server'])) $sql .= 'download_server = ' . sql_value($result['download_server'], "string") . ', '; else $sql .= 'download_server = null, ';
   //if (isset($result['print_url']))    $sql .= 'print_url    = ' . sql_value($result['print_url'],    "string") . ', '; else $sql .= 'print_url    = null, ';
   if (isset($result['image_url']))       $sql .= 'image_url       = ' . sql_value($result['image_url'],       "string") . ', '; else $sql .= 'image_url       = null, ';
   if (isset($result['date']))            $sql .= 'date            = ' . sql_value($result['date'],            "string") . ', '; else $sql .= 'date            = null, ';
   if (isset($result['release_date']))    $sql .= 'release_date    = ' . sql_value($result['release_date'],    "string") . ', '; else $sql .= 'release_date    = null, ';
   if (isset($result['kind']))            $sql .= 'kind            = ' . sql_value($result['kind'],            "string") . ', '; else $sql .= 'kind            = null, ';
   if (isset($result['language']))        $sql .= 'language        = ' . sql_value($result['language'],        "string") . ', '; else $sql .= 'language        = null, ';
   if (isset($result['size']))            $sql .= 'size            = ' . sql_value($result['size'],            "string") . ', '; else $sql .= 'size            = null, ';
   if (isset($result['magazine_type']))   $sql .= 'magazine_type   = ' . sql_value($result['magazine_type'],   "string") . ', '; else $sql .= 'magazine_type   = null, ';
   if (isset($result['page_nb']))         $sql .= 'page_nb         = ' . sql_value($result['page_nb'],         "string") . ', '; else $sql .= 'page_nb         = null, ';
   if (isset($result['magazine_nb']))     $sql .= 'magazine_nb     = ' . sql_value($result['magazine_nb'],     "string") . ', '; else $sql .= 'magazine_nb     = null, ';
   if (isset($result['old_issue']))       $sql .= 'old_issue       = ' . sql_value($result['old_issue'],       "string") . ', '; else $sql .= 'old_issue       = null, ';
   if (isset($result['isbn']))            $sql .= 'isbn            = ' . sql_value($result['isbn'],            "string") . ', '; else $sql .= 'isbn            = null, ';
   if (isset($result['quality']))         $sql .= 'quality         = ' . sql_value($result['quality'],         "string") . ', '; else $sql .= 'quality         = null, ';
   if (isset($result['info'])) {
      $info = implode(', ', $result['info']);
      $sql .= 'info = ' . sql_value($info, "string");
   } else
      $sql .= 'info = null';

   $sql .= " where print_id = $print_id";

   //LoggLine($sql);

   $db->query($sql);

   $sql = "delete from $table_print_url where print_id = $print_id";
   $db->query($sql);

   foreach ($result['print_url'] as $print_url) {
      $sql = "insert into $table_print_url ( print_id, server, url ) values ( ";
      $sql .= "$print_id, ";
      $sql .= sql_value($print_url['server'],  "string") . ', ';
      $sql .= sql_value($print_url['url'],     "string") . ' )';
      $db->query($sql);
   }
}

function get_print_id($db, $table_print, $url_code) {
   //$table_print = 'print';
   $row = $db->query_row("select print_id from $table_print where url_code = " . sql_value($url_code, "string"));
           //. 'site = ' . sql_value($site, "string")
      //. ' and type = ' . sql_value($type, "string")
      //. ' and name = ' . sql_value($name, "string"));
   if ($row)
      return $row['print_id'];
   else
      return null;
}

function insert_print($db, $table_print, $site, $type, $name, $url_code, $url_download_page) {
   //$table_print = 'print';
   $sql = "insert into $table_print (site, type, name, ts, url_code, url_download_page) values ( "
      . sql_value($site,              "string") . ', '
      . sql_value($type,              "string") . ', '
      . sql_value($name,              "string") . ', '
      . 'current_timestamp(), '
      . sql_value($url_code,          "string") . ', '
      . sql_value($url_download_page, "string") . ' )';
   $db->query($sql);
}

function get_magazines($html) {
   //$dom = new DOMDocument();
   ////$dom->encoding = 'UTF-8';
   //libxml_use_internal_errors(true);
   //libxml_clear_errors();
	//$dom->loadHTML($html);
	//$errors = libxml_get_errors();
   //libxml_use_internal_errors(false);
   //$xdom = new DOMXPath($dom);

   $xdom = get_dom_xpath($html);
   $magazines = array();

   // <div class="azindex">
   // <ul>
   // <li><h2><a href="#azindex-1" title="Return to the top">0</a></h2></li>
   // <li><a href="http://www.vosbooks.com/3615-revues-magazines/01-informatique-business-et-technologie-12-january-2011-france-megaupload.html"><span class="head">01 Informatique Business et Technologie - 12 January 2011 (France) </span></a></li>
   // <li><a href="http://www.vosbooks.com/3626-revues-magazines/01-informatique-business-et-technologie-12-janvier-2011-2-megaupload.html"><span class="head">01 Informatique Business et Technologie 12 Janvier 2011</span></a></li>

   //$query = "//tr[@class='WindowTableCell1']";
   $query = "//div[@class='azindex']/ul/li/a";
   //$query = "//div[@class='azindex']";
   $nodes = $xdom->query($query);
   //if (!$nodes)
      //LoggLine("query \"$query\" : " . str_var_dump($nodes));
   //else {
   if ($nodes) {
      //LoggLine("magazines : {$nodes->length}");
      //$i = 0;
      foreach ($nodes as $node) {
         //print_node($node);
         $href = $node->attributes->getNamedItem("href");
         $url = $href->value;

         $url_code = get_url_code($url);

         $magazines[] = array('name' => $node->nodeValue, 'url_download_page' => $url, 'url_code' => $url_code);
         //LoggLine("href      : {$href->value}");
         //LoggLine("nodeValue : {$node->nodeValue}");
         
         //if (++$i == 10) break;
      }
   }
   return $magazines;
}

function get_url_code($url) {
   $r = preg_match('@^http\://www\.vosbooks\.com/(.+)/@', $url, $matches);
   if ($r !== 1) {
      //LoggLine("error in url \"$url\"");
      return null;
   }
   return $matches[1];
}

function control_analyse_vosbooks_01() {
   $log_file = '_files_\vosbooks\log_control_magazines_analyse_01_99.txt';
   delete_file($log_file);


   // pb
   test_analyse_vosbooks_01($log_file, utf8_encode('21 Décembre 2012 Magazine N°1 Édition 2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Le Vif/L\'Express - Fevrier 2011'));
   // pb

   // ok
   test_analyse_vosbooks_01($log_file, utf8_encode('Micro Hebdo Fiches pratiques Decembre 2010Janvier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Marianne N°713-714 du 18 au 31Decembre 2010 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('MAG .Psd Photoshop N°01(43) Janvier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Science Vie Junior 259 Avril 2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Xbox World 360 Fevrier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Le Vif - L\'Express N°3108 du 28 janvier au 03 février 2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('IPhone Life - Vol 3 N°2 2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Pack Archaeology Magazine Full Year 2011 - 6 Issues'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Daily Mail 2011.01.03'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Action Auto Moto Hors-Serie N°69 Été 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Stig Med1a Issue 1.1.11 January 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Photo no.476 Janvier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('PH magazine Issue #3'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Webuser #255 16 December 2010'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Auto Express – New Year Special 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Action Auto Moto N°189 Juin 2011 Mu'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Computer Shopper  February (2011)'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Closer N°291 - 8/14 janvier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Web Pocket n°8 (Mars/avril/mai 2011)'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Creanum N°144 Sep/Octobre 2010'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Action Auto Moto N 185 [Janv à Fev 2011]'));
   test_analyse_vosbooks_01($log_file, utf8_encode('60 Millions de Consommateurs Hors-Serie N°154 Mars Avril 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('01 Informatique Business et Technologie - 12 January 2011 (France) '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Bakchich Hebdo N 52 Vendredi 24 Decembre / Jeudi 13 Janvier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Btorrent MAGAZINE N°5 NOVEMBRE JANVIER'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Creanum N°145 Nov-Decembe 2010/Janvier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('L\'Auto Journal Évasion & 4x4 N°56 Juin Juillet Aout 2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('LE JOURNAL DE MICKEY N3054 29 Décembre 2010 au 04 Janvier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Maison Cote Est N°53 Sept/Decembre 2010'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Micro Hebdo N662_663 23 Décembre au 5 janvier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Montres N°79 - Hiver 2010/2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Médias N°27 Hiver 2010/2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Neo Planete N°19 Mars 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('NZV8 Fevrier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Pirate Informatique N°05 Juillet et Aout 2010'));
   test_analyse_vosbooks_01($log_file, utf8_encode('PSM3 Février 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Question Micro Jan/Fev/Mars 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Question Micro N°21 Janvier/Fevrie/Mars 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Solutions et Logiciels N16 Nov-Dec 2010 Janv 2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('The Economist 1st January-7th January 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Voici N°1208 31 Decembre 2010 au 7 Janvier 2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('France Dimanche N°3356 24 au 30 Dec 2010'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Ici Paris N3416 21 au 27 dec 2010'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Maison Cote Est N°53 Sept/Decembre 2010'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Paris Match N°3210,25 Novembre/1 Decembre 2010'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Web Pocket N°7 Dec 2010 à Fev 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('60 Millions de Consommateurs Hors Série N°109 Novembre Décembre 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Elle à Table N°77 Juillet Août 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Gagner au Quinte N°143 juin 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Images du Monde N°27 Mai 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Images du Monde Point de Vue N°26 Février Mars Avril 2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('L\'Essentiel du Camping-Car N°6 mai juin juillet 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('L\'Essentiel Du Mobile N36 Fevrier 2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Le Journal du Dimanche N°3338 Dimanche 2 Janvier 2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Le Spectacle du Monde N°573 Decembre 2010 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Micro Hebdo N662_663 23 Décembre au 5 janvier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Soldats du Feu N°41 Nov/Dec 2010'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Valeurs Actuelles N°3865 23 de 2010 au 5 Janv 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('01 Informatique, Business et Technologie - 3 Fevrier 2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Aquarium a la Maison N°83 Janvier/Fevrier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Bakchich Hebdo N°51 Semaine du 17/23 décembre 2010'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Computer Active issue 335 23 December to 05 January 2010 (UK)'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Creanum N°145 Nov-Decembe 2010/Janvier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Guide d\'achat numérique HS N°1 Eté 2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Hacker News MagazineN° 16'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Les Inrockuptibles N°814 du 6 au 12 juillet 2011 + Supplément Paris l\'anti-guide'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Maison Cote Est N°53 Sept/Decembre 2010'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Micro Hebdo n°662-663 + HS + Fiches Pratiques'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Pack Micro Hebdo {N° 558 à 568} '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Le Point N°1997/N°1998-Hebdo des Jeudis Le 23 et 30 Decembre 2010'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Automobile - Fevrier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Pack PC Trucs et Astuces N°1 N°2 N°3 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Micro Hebdo N°662-N°663 du 23 Decembre au 05 Janvier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Micro Hebdo N°678 & N°679 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Autosport Magazine, December 16th 2010'));
   test_analyse_vosbooks_01($log_file, utf8_encode('The Economist 1st January-7th January 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Micro Hebdo N662_663 23 Décembre au 5 janvier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Micro Hebdo N°678 & N°679 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Micro Hebdo N°687 N°688 N°689 N°690 & N°691 du 16 Juin au 20 Juillet 2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Pirate Informatique N°06 septembre – octobre 2010'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Shooting Magazine N°13 decembre 2010 – janvier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Valeurs Actuelles N°3865 23 de 2010 au 5 Janv 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('[La guerre des serpents-4]Les fragments d’une couronne brisee(1998)'));
   test_analyse_vosbooks_01($log_file, utf8_encode('[HF] L\'Essentiel Du Mobile N36 Fevrier 2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Programmez magazine 7 ans de numéros - 74 numéros'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Pirate Informatique N°05 Juillet et Aout 2010'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Revue Technique Peugeot 406'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Revue Technique Voxan 1000'));
   test_analyse_vosbooks_01($log_file, utf8_encode('The Economist 1st January-7th January 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('TOP 500 sites internet N°6 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('MAG .Psd Photoshop N°01(43) Janvier 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Mes petites mains Le N 2 / Noel 2010'));
   test_analyse_vosbooks_01($log_file, utf8_encode('MSDN Magazine 2010 Full Year Collection'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Tech in Style CES 2011 Special'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Le Guide 2011 Chauffages et Cheminées'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Excel 2010 Fonctions et Formules'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Auto Plus Hors Série N°7 Guide 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Auto Plus Hors Série N°29 Le guide 2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Auto Express – New Year Special 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Arabian Horse Edition 2011'));
   test_analyse_vosbooks_01($log_file, utf8_encode('Auto Plus Hors-Serie - Occasions 2011 N°30 du 18 fevrier 2011 '));
   test_analyse_vosbooks_01($log_file, utf8_encode('Auto Plus Hors Série N°29 Le guide 2011 '));
   // ok

   append_to_file($log_file, "************************************************************************************************************************" . CRLF);
}

function test_get_values_01($name) {
   LoggLine("get_values : \"$name\"");
   $values = array();
   $values['name'] = $name;
   $name2 = get_values($name, $values);
   $name2 = uft8_string_normalize($name);
   //$name2 = strtolower($name);
   $name2 = mb_strtolower($name2, "utf8");
   $values['print_paper'] = $name2;
   LoggLine("result     : \"$name2\"");
   LoggLine(str_var_dump($values));
}

function test_analyse_vosbooks_01($log_file, $name) {
   LoggLine("get_values : \"$name\"");
   $values = array();
   $values['name'] = $name;
   $name2 = get_values($name, $values);
   $name2 = uft8_string_normalize($name2);
   //$name2 = strtolower($name2);
   $name2 = mb_strtolower($name2, "utf8");
   $values['print_paper'] = $name2;
   log_magazines($log_file, $values);
}

function test_get_attribs_01($s) {
   LoggLine("get_attribs : \"$s\"");
   $s = get_attribs($s, $values, $attrib_number);
   LoggLine("result      : \"$s\"");
   LoggLine(str_var_dump($values));
}

function test_database_01() {
   //$host = 'localhost';
   //$database = 'print';
   $db = open_db();
   $db->close();
}

function test_DOMDocument_01() {
   $url = 'http://www.vosbooks.com/liste-des-magazines';

   //$ch = curl_init();
   //curl_setopt($ch, CURLOPT_URL, $url);
   //curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
   //$html = curl_exec($ch);
   //curl_close($ch);

   LoggLine("load url \"$url\"");
   $html = load_url($url);

   $check = check_utf8($html, $corrected_html, $result);
   if (!$check) {
      $html = $corrected_html;
      LoggLine("remove {$result['nb_bad_bytes']} bad utf8 char");
   }
   write_to_file('_files_\\vosbooks\\liste-des-magazines_01.html', $html);

   //$dom = new DOMDocument();
   //$dom->encoding = 'UTF-8';
   //libxml_use_internal_errors(true);
   //libxml_clear_errors();
	//$dom->loadHTML($html);
	//$errors = libxml_get_errors();
   //libxml_use_internal_errors(false);

   $dom = get_dom($html);

   $html2 = $dom->saveHTML();
   write_to_file('_files_\\vosbooks\\liste-des-magazines_02.html', $html2);
}

function test_DOMDocument_02($html_file, $out_file) {
   $html = read_file($html_file);

   //$dom = new DOMDocument();
   //$dom->encoding = 'UTF-8';
   //libxml_use_internal_errors(true);
   //libxml_clear_errors();
	//$dom->loadHTML($html);
	//$errors = libxml_get_errors();
   //libxml_use_internal_errors(false);

   $dom = get_dom($html);

   $html2 = $dom->saveHTML();
   write_to_file($out_file, $html2);
}

function test_check_utf8_01($file, $file2 = null) {
   $data = read_file($file);
   $check = check_utf8($data, $corrected_string, $result);
   LoggLine("check utf8 file \"$file\" : " . str_var_dump($check));
   LoggLine(str_var_dump($result));
   //LoggLine("utf8 remove bad chars file \"$file\" :");
   //$data2 = utf8_remove_bad_chars($data, $result);
   //LoggLine(str_var_dump($result));
   if ($file2 != null) {
      LoggLine("write new utf8 in file \"$file2\"");
      write_to_file($file2, $corrected_string);
   }
}

function print_node($node, $root_node = null) {
   // $node->nodeValue == $node->textContent
   $path = get_node_path($node, $root_node);
   Logg('node : ' . $path);
   switch ($node->nodeType) {
      case XML_ELEMENT_NODE:
         $attributes = dom_get_string_attributes($node);
         Logg(" ($attributes)");
         break;
      case XML_ATTRIBUTE_NODE:
         Logg(" ($node->nodeValue)");
         break;
      case XML_TEXT_NODE:
      //textContent
         Logg(" ($node->nodeValue)");
         break;
      default:
         $type = get_node_type($node);
         Logg(" ($type $node->nodeValue)");
   }
   LoggLine();
}

function get_node_type($node) {
   switch($node->nodeType) {
      case XML_ELEMENT_NODE:             return "Element";
      case XML_ATTRIBUTE_NODE:           return "Attribute";
      case XML_TEXT_NODE:                return "Text";
      case XML_CDATA_SECTION_NODE:       return "CData";
      case XML_ENTITY_REF_NODE:          return "EntityRef";
      case XML_ENTITY_NODE:              return "Entity";
      case XML_PI_NODE:                  return "PI";
      case XML_COMMENT_NODE:             return "Comment";
      case XML_DOCUMENT_NODE:            return "Document";
      case XML_DOCUMENT_TYPE_NODE:       return "DocumentType";
      case XML_DOCUMENT_FRAG_NODE:       return "DocumentFrag";
      case XML_NOTATION_NODE:            return "Notation";
      case XML_HTML_DOCUMENT_NODE:       return "HtmlDocument";
      case XML_DTD_NODE:                 return "DTD";
      case XML_ELEMENT_DECL_NODE:        return "ElementDecl";
      case XML_ATTRIBUTE_DECL_NODE:      return "AttributeDecl";
      case XML_ENTITY_DECL_NODE:         return "EntityDecl";
      case XML_NAMESPACE_DECL_NODE:      return "NamaspaceDecl";
   }
}

function get_node_path($node, $root_node = null) {
   $path = '';
   //while (isset($node) && $node->nodeName != '#document') {
   while (isset($node) && $node->nodeType != XML_HTML_DOCUMENT_NODE) {
      $sibling_count = true;
      if ($node === $root_node) $sibling_count = false;
      switch ($node->nodeType) {
         case XML_ELEMENT_NODE:
            if ($sibling_count) {
               $count = get_node_sibling_count($node) + 1;
               $name = $node->nodeName . "[$count]";
            } else
               $name = $node->nodeName;
            break;
         case XML_ATTRIBUTE_NODE:
            $name = '@' . $node->nodeName;
            break;
         default:
            //$name = "????$node->nodeName????";
            $name = null;
      }
      if ($node === $root_node) {
         if ($name != null)
            $path = $name . $path;
         break;
      }
      if ($name != null)
         $path = '/' . $name . $path;
      $node = $node->parentNode;
      //if ($node == $root_node) break;
   }
   if ($path == '') $path = '/';
   return $path;
}

function get_node_sibling_count($node) {
   if ($node->nodeType !== XML_ELEMENT_NODE) return -1;
   $name = $node->nodeName;
   $count = 0;
   while (true) {
      $node = $node->previousSibling;
      if (!isset($node)) break;
      if ($node->nodeName === $name && $node->nodeType === XML_ELEMENT_NODE) $count++;
   }
   return $count;
}

//function open_db($database, $host = 'localhost') {
function open_db() {
   global $g_db_database, $g_db_host;
   $db = new mysql($g_db_host, "mysql", "root", "beuzserv");
	db_open($db);
   if (!$db->database_exist($g_db_database)) {
      LoggLine("create database $g_db_database");
      $db->query("create database $g_db_database");
   }
   $db->close();
   $db = new mysql($g_db_host, $g_db_database, "root", "beuzserv");
	db_open($db);
	return $db;
}

function db_open($db) {
   LoggLine("connect to {$db->host} {$db->database}");
	$db->open();
}

/*******************************************************************************************************************************************************************
function get_download_page_info_old($html) {

   //    <div class="entry">   //    ...   //    <table class="aligncenter">   //    <tbody>   //    <tr rowspan="5">   //    <td rowspan="5" width="200" height="320" bgcolor="#ffffff">   // ** <img title="Micro Hebdo N°671 du 24 février au 2 mars 2011" src="http://img54.xooimage.com/files/f/1/3/mh671-25ecb77.jpg" alt="[HF] Micro Hebdo N°671 du 24 février au 2 mars 2011  Megaupload" width="221" height="320" />   //    </td>   //    </tr>   //    <tr>   //    <td width="400">
   //    <img src="../image/Description.png" alt="[HF] Micro Hebdo N°671 du 24 février au 2 mars 2011  Megaupload" width="200" height="30" title="[HF] Micro Hebdo N°671 du 24 février au 2 mars 2011  Megaupload" />
   //    </p><p>
   // ** Micro Hebdo N°671 du 24 février au 2 mars 2011
   //    </td>
   //    </tr>
   //    <tr>
   // ** <td width="350"><strong> Genre : </strong>Informatique</td>
   //    </tr>
   //    <tr>
   // ** <td width="350"><strong>Type : </strong> Pdf</td>
   //    </tr>
   //    <tr>
   // ** <td width="350"><strong> Hébergeur : </strong> Hotfile</td>
   //    </tr>
   //    </tbody>
   //    </table>
   //    <p style="text-align: center;"><span id="more-6627"></span></p>
   //    <p style="text-align: center;"><img src="../image/infos%20fichier.png" alt="[HF] Micro Hebdo N°671 du 24 février au 2 mars 2011  Megaupload"  title="[HF] Micro Hebdo N°671 du 24 février au 2 mars 2011  Megaupload" /></p>
   //    <p style="text-align: center;">
   // ** <strong> Date Magazine : </strong> du 24 février au 2 mars 2011<br />
   // ** <strong> Nombre de Pages : </strong> 60<br />
   // ** <strong> Langue : </strong> Français<br />
   // ** <strong>Taille  : </strong> 50.21 Mo<br />
   //    <strong> </strong><br />
   //    <img src="../image/telechargement.png" alt="[HF] Micro Hebdo N°671 du 24 février au 2 mars 2011  Megaupload"  title="[HF] Micro Hebdo N°671 du 24 février au 2 mars 2011  Megaupload" />
   //    </p>
   //    <p style="text-align: center;">
   //    <strong>
   // ** <a href="http://hotfile.com/dl/107383970/a90680c/Micro_Hebdo_N671_WwW.VosBooks.com.pdf.html" target="_blank"> Télécharger Micro Hebdo N°671 du 24 février au 2 mars 2011 Sur HotFile </a>
   //    </strong>
   //    </p>
   //    <p style="text-align: center;">&nbsp;</p>
   //    <center>   //    <p><strong>[HF] Micro Hebdo N°671 du 24 février au 2 mars 2011 50x plus rapide avec Usenet.nl</strong></p>   //    <p><a href="http://www.vosbooks.com/image/Member-VIP.html" target="_blank" ><strong>Telecharger [HF] Micro Hebdo N°671 du 24 février au 2 mars 2011 50x plus rapidement avec usenet.nl</strong></a></p>   //    <p class="submeta"><strong>E-books</strong>    //    :
   //    <a href="http://www.vosbooks.com/tag/magazine-micro-hebdo-n%c2%b0671-du-24-fevrier-au-2-mars-2011" rel="tag">Magazine Micro Hebdo N°671 du 24 février au 2 mars 2011</a>, <a href="http://www.vosbooks.com/tag/magazine-micro-hebdo-n%c2%b0671-du-24-fevrier-au-2-mars-2011-gratuit" rel="tag">Magazine Micro Hebdo N°671 du 24 février au 2 mars 2011 gratuit</a>, <a href="http://www.vosbooks.com/tag/micro-hebdo-n%c2%b0671-du-24-fevrier-au-2-mars-2011-magazine-megaupload" rel="tag">Micro Hebdo N°671 du 24 février au 2 mars 2011 Magazine Megaupload</a>, <a href="http://www.vosbooks.com/tag/telecharger-magazine-micro-hebdo-n%c2%b0671-du-24-fevrier-au-2-mars-2011" rel="tag">Télécharger Magazine Micro Hebdo N°671 du 24 février au 2 mars 2011</a>, <a href="http://www.vosbooks.com/tag/telecharger-micro-hebdo-n%c2%b0671-du-24-fevrier-au-2-mars-2011-megaupload" rel="tag">Télécharger Micro Hebdo N°671 du 24 février au 2 mars 2011 Megaupload</a>
   //    <br />
   //    </p>   //    </div>

   $xdom = get_dom_xpath($html);

   $result = array();

   //<div class="entry">
   $nodes = $xdom->query("//div[@class='entry']");
   if ($nodes->length === 0) {
      LoggLine('<div class="entry"> not found');
      return null;
   }
   if ($nodes->length > 1) {
      LoggLine('to many <div class="entry">');
      return null;
   }
   $div = $nodes->item(0);

   // <table class="aligncenter">   $nodes = $xdom->query("table", $div);
   if ($nodes->length === 0) {
      LoggLine('<div class="entry"> <table> not found');
      return null;
   }
   if ($nodes->length > 1) {
      LoggLine('to many <div class="entry"> <table>');
      return null;
   }
   $table = $nodes->item(0);


   $nodes = $xdom->query("(.//img)[position()=1]/@src", $table);
   if ($nodes->length == 1) {
      $result['image_url'] = $nodes->item(0)->value;
      //LoggLine("image : $img");
   }


   $nodes = $xdom->query("descendant::text()", $table);
   $first = true;
   $var_name = null;
   foreach ($nodes as $node) {
      $s = trim($node->nodeValue);
      if ($s != '') {
         if ($first) {
            $result['title'] = $s;
            $first = false;
         } else {
            //$r = preg_match('@^([^:^\s]+)\s*:$@', $s, $matches);
            $r = preg_match('@^([^:]+):$@', $s, $matches);
            if ($r == 1)
               $var_name = trim($matches[1]);
            else if ($var_name != null) {
               $result[$var_name] = $s;
               $var_name = null;
            }
         }
      }
   }



   //$nodes = $xdom->query("descendant::text()", $div);
   $nodes = $xdom->query("following-sibling::* /descendant::text()", $table);
   $var_name = null;
   foreach ($nodes as $node) {
      $s = trim($node->nodeValue);
      if ($s != '') {
         $r = preg_match('@^([^:]+):$@', $s, $matches);
         if ($r == 1)
            $var_name = trim($matches[1]);
         else if ($var_name != null) {
            $result[$var_name] = $s;
            $var_name = null;
         }
      }
   }
   

   $nodes = $xdom->query("following-sibling::* /descendant::a/@href", $table);
   if ($nodes->length > 1) {
      $result['download_url'] = $nodes->item(0)->value;
   }

   //LoggLine(str_var_dump($result));
   return $result;

   //foreach ($nodes as $node) {
      //print_node($node);
   //}
}
*******************************************************************************************************************************************************************/


/*******************************************************************************************************************************************************************
function check_utf8_old($str, &$errors) {
   // from http://fr.wikipedia.org/wiki/UTF-8
   // 0xxxxxxx                   00-7F (000-127)                         1 octet  codant  1 à  7 bits
   // 110xxxxx + 1 x 10xxxxxx    C0-DF (192-223) + 1 x 80-BF (128-191)   2 octets codant  8 à 11 bits  5 + 1 x 6 = 11
   // 1110xxxx + 2 x 10xxxxxx    E0-EF (224-239) + 2 x 80-BF (128-191)   3 octets codant 12 à 16 bits  4 + 2 x 6 = 16
   // 11110xxx + 3 x 10xxxxxx    F0-F7 (240-247) + 3 x 80-BF (128-191)   4 octets codant 17 à 21 bits  3 + 3 x 6 = 21
   // 110xxxxx : C0-DF

   $res = true;
   $errors = array();

   $len = strlen($str);
   $line = 1;
   $col = 1;
   for($i = 0; $i < $len; $i++) {
      $c = ord($str[$i]);
      if ($c > 127) {
         if ($c < 192 || $c > 247) { // 128-191 248-255
            $errors[] = "first byte can't be between 128-191 and 248-255 : char $c on line $line column $col position $i";
            $res = false;
         } else {
            if ($c > 239) $bytes = 3;          // 240-247 : code on 4 bytes
            else if ($c > 223) $bytes = 2;     // 224-239 : code on 3 bytes
            else $bytes = 1;                   // 192-223 : code on 2 bytes
            if ($i + $bytes >= $len) {
               $missing = $i + $bytes - $len;
               $errors[] = "missing $missing bytes : char $c on line $line column $col position $i";
               $res = false;
               break;
            }
            $bytes_code = $bytes + 1;
            while ($bytes > 0) {
               $i++;
               $b = ord($str[$i]);
               if ($b < 128 || $b > 191) {
                  $errors[] = "error in $bytes_code bytes code, extra byte should be between 128-191 : char $c byte $b on line $line column $col position $i";
                  $res = false;
                  $i--;
                  break;
               }
               $bytes--;
            }
         }
      } else if ($c == 10) {
         $line++;
         $col = 0;
      }
      $col++;
   }
   return $res;
}
*******************************************************************************************************************************************************************/
?>
