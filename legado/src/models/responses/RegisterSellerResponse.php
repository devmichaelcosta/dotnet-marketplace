<?php   

    namespace models\responses;
    
    class RegisterSellerResponse 
    {
        private $_success;
        private $_msg;

        public function __construct(bool $success, string $msg)
        {
            $this->_success = $success;
            $this->_msg = $msg;
        }
     
        public function getMsg()
        {
            return $this->_msg;
        }

        public function getSuccess()
        {
            return $this->_success;
        }

    }