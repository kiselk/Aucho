<?php 

?>
<html>
  <head>
    <title>Scatterplot</title>
    <link type="text/css" rel="stylesheet" href="ex.css?3.2"/>
    <script type="text/javascript" src="protovis-r3.2.js"></script>
    <script type="text/javascript">
var data = pv.range(100).map(function(x) {
    return {x: x, y: Math.random(), z: Math.pow(10, 2 * Math.random())};
  });

var data2 = [

<?php
$min_timestamp=99999999;
$max_timestamp=0;
$min_price=99999999;
$max_price=0;
    $link = mysql_connect("localhost", "onmap", "onmappassword")
        or die("Could not connect : " . mysql_error());
    mysql_select_db("onmap") or die("Could not select database");
    $query = "SELECT timestamp, price, count FROM `all`";
    $result = mysql_query($query) or die("Query failed : " . mysql_error());
     $new=1;
while ($line = mysql_fetch_array($result, MYSQL_ASSOC)) {
if ($new==0) {print ","; } else { $new = 0; }
$timestamp=$line["timestamp"];
$price = $line["price"];
$count = $line["count"];

if ($min_timestamp>$timestamp) $min_timestamp=$timestamp;
if ($max_timestamp<$timestamp) $max_timestamp=$timestamp;
if ($min_price>$price) $min_price=$price;
if ($max_price<$price) $max_price=$price;


 print "{x: " . $timestamp . ", y: " . $price . ", z: " . $count . "}";
    }
    mysql_free_result($result);
    mysql_close($link);
?>
];
  </script>
    <style type="text/css">

#fig {
  width: 800px;
  height: 425px;
}

    </style>
  </head>
  <body><div id="center"><div id="fig">
    <script type="text/javascript+protovis">

/* Sizing and scales. */
var w = 770,
    h = 400,
    x = pv.Scale.linear(<?php print $min_timestamp-10;?>, <?php print $max_timestamp+10;?>).range(0, w),
    y = pv.Scale.linear(<?php print $min_price-10;?>, <?php print $max_price+10;?>).range(0, h),
    c = pv.Scale.log(1, 100).range("orange", "brown");

/* The root panel. */
var vis = new pv.Panel()
    .width(w)
    .height(h)
    .bottom(20)
    .left(20)
    .right(10)
    .top(5);

/* Y-axis and ticks. */
vis.add(pv.Rule)
    .data(y.ticks())
    .bottom(y)
    .strokeStyle(function(d) d ? "#eee" : "#000")
  .anchor("left").add(pv.Label)
    .visible(function(d) d > <?php print $min_price-10;?> && d < <?php print $max_price+10;?>)
    .text(y.tickFormat);

/* X-axis and ticks. */
vis.add(pv.Rule)
    .data(x.ticks())
    .left(x)
    .strokeStyle(function(d) d ? "#eee" : "#000")
  .anchor("bottom").add(pv.Label)
    .visible(function(d) d > <?php print $min_timestamp-10;?> && d < <?php print $max_timestamp+10;?>)
    .text(x.tickFormat);

/* The dot plot! */
vis.add(pv.Panel)
    .data(data2)
  .add(pv.Dot)
    .left(function(d) x(d.x))
    .bottom(function(d) y(d.y))
    .strokeStyle(function(d) c(d.z))
    .fillStyle(function() this.strokeStyle().alpha(.2))
    .size(function(d) d.z)
    .title(function(d) d.z.toFixed(1));

vis.render();

    </script>
  </div></div></body>
</html>