<?php

    namespace infra;

    class Logger
    {
        public static function write($log)
        {
            $fh = fopen("logs.txt", "a");
            fwrite($fh, $log . "\n");
            fclose($fh);
        }
    }