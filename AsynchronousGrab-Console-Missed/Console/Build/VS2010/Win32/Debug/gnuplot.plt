reset
set title "Time"
set xlabel 'Iteration'
set ylabel 'Milliseconds'
plot "truking-log.csv" u 2:6 w l title "FirstFrame" lc "blue", "truking-log.csv" u 2:8 w l title "16Frames" lc "#FF0000", "truking-log.csv" u 2:4 w l title "Temperature" lc "orange", "truking-log.csv" u 2:10 w l title "ImageProceTime" lc "gray"
