<?php
namespace models;

class JsonSuccess extends JsonAjax
{
    public function __construct($msg){
        $this->msg = $msg;
        $this->success = true;
    }

    
}
