<?php
namespace models;

class JsonAjax 
{
    public $msg;
    public $success;


    /**
     * Get the value of success
     */ 
    public function getSuccess()
    {
        return $this->success;
    }

    /**
     * Get the value of msg
     */ 
    public function getMsg()
    {
        return $this->msg;
    }
    
    public function jsonSerialize()
    {
        return [

            'msg' => $this->getMessage(),
            'success'=>$this->getSuccess()

        ];
    }
}