<?php

    namespace models;
    use models\ProductOrder;

    class OrderDetailsViewModel
    {
        private $_orderId;
        private $_createAt;
        private $_total;
        private $_products;
        private $_address;
        private $_name;
        private $_state; 
        private $_city; 
        private $_cep; 
        private $_cpf; 
        private $_neighborhood; 
        private $_complement; 

        public function __construct($orderId, $createAt, $total)
        {
            $this->_orderId = $orderId;
            $this->_createAt = $createAt;
            $this->_total = $total;
        }
        
        public function setAddress($address){
            $this->_address = $address;
            return $this;
        }
        public function setState($state){
            $this->_state = $state;
            return $this;
        }

        
        public function setName($name){
            $this->_name = $name;
            return $this;
        }

        public function setProducts($products)
        {
            $this->_products = $products;
            return $this;
        }
        public function setNeighborhood($neighborhood)
        {
            $this->_neighborhood = $neighborhood;
            return $this;
        }
        public function setCep($cep)
        {
            $this->_cep = $cep;
            return $this;
        }
        public function setCpf($cpf)
        {
            $this->_cpf = $cpf;
            return $this;
        }
        public function setCity($city)
        {
            $this->_city = $city;
            return $this;
        }
        public function setComplement($complement)
        {
            $this->_complement = $complement;
            return $this;
        }
        public function getProducts(){
            return $this->_products;
        }
        public function getOrderId(){
            return $this->_orderId;
        }
        public function getTotal(){
            return $this->_total;
        }
        
        public function getTotalFormatted()
        {
            return "R$ ".  number_format($this->_total,2,",",".");
        }
        public function getName(){
            return $this->_name;
        }
        public function getAddress(){
            return $this->_address;
        }
        public function getState(){
            return $this->_state;
        }
        public function getNeighborhood(){
            return $this->_neighborhood;
        }
        public function getCity(){
            return $this->_city;
        }
        public function getCpf(){
            return $this->_cpf;
        }
        public function getCep(){
            return $this->_cep;
        }
        public function getComplement(){
            return $this->_complement;
        }
        public function getCreateAtFormatted() {
            
            return date("d-m-Y H:i:s", strtotime( $this->_createAt ) );
        }
    }

?>