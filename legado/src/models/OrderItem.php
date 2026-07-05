<?php
    namespace models;

    class OrderItem
    {
        private $_orderItemId;
        private $_orderId;
        private $_productId;
        private $_qtd;

        public function __construct($orderItemId,$orderId,$productId,$qtd)
        {
           $this->_orderId =$orderId;
           $this->_orderItemId =$orderItemId;
           $this->_productId =$productId;
           $this->_qtd =$qtd;
            
        }
        public function getOrderId(){
            return $this->_orderId;
        }
        public function getProductId(){
            return $this->_productId;
        }
        public function getQtd(){
            return $this->_qtd;
        }
    }