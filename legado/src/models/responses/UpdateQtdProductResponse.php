<?php   
    namespace models\responses;
    
    class UpdateQtdProductResponse 
    {
        public function __construct(bool $success, string $msg, int $stock, int $qtd, $finalValue, $subTotal)
        {
            $this->_success = $success;
            $this->_msg = $msg;
            $this->_stock = $stock;
            $this->_qtd = $qtd;
            $this->_finalValue = $finalValue;
            $this->_subTotal = $subTotal;
        }

        private $_success;
        private $_msg;
        private $_stock;
        private $_qtd;
        private $_finalValue;
        private $_subTotal;

        public function getSubTotal()
        {
            return $this->_subTotal;            
        }

        public function getFinalValue()
        {
            return $this->_finalValue;
        }

        public function getQtd()
        {
            return $this->_qtd;
        }
        
        public function getMsg()
        {
            return $this->_msg;
        }

        public function getStock()
        {
            return $this->_stock;
        }

        public function getSuccess()
        {
            return $this->_success;
        }

    }