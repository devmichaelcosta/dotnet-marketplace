<?php
namespace models;

class JsonError extends JsonAjax
{
    private $errors;
    
    /**
     * Set the value of errors
     *
     * @return  self
     */ 
    public function setErrors($errors)
    {
        $this->errors = $errors;
        return $this;
    }

    public function __construct($msg){
        $this->msg = $msg;
        $this->success = false;
    }

    /**
     * Get the value of errors
     */ 
    public function getErrors()
    {
        return $this->errors;
    }
}