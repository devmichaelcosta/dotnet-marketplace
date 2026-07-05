<?php
    namespace models;

    class Order
    {
        private $_orderId;
        private $_total;
        private $_name;
        private $_cardOwnerName;
        private $_cardNumber;
        private $_createdAt;
        private $_userId;
        private $_stateId;
        private $_expirationDate;
        private $_address;
        private $_neighborhood;
        private $_cep;
        private $_city;
        private $_cpf;
        private $_orderItens;
        private $_complement;

        public function __construct(
                $orderId, $userId, $total, $name, 
                $cardOwnerName, $cardNumber, $expirationDate, $cvv,
                $cep, $cpf,
                $address,$neighborhood,$city,$stateId,$complement,
                $orderItens

            )
        {
            $this->_orderId = $orderId;
            $this->_userId = $userId;
            $this->_total = $total;
            $this->_name = $name;
            
            $this->_cardOwnerName = $cardOwnerName;
            $this->_cardNumber = $cardNumber;
            $this->_expirationDate = $expirationDate;
            $this->_cvv = $cvv;

            $this->_cpf = $cpf;
            $this->_cep = $cep;
            $this->_address = $address;
            $this->_neighborhood = $neighborhood;
            $this->_city = $city;
            $this->_stateId = $stateId;
            $this->_complement = $complement;

            $this->_orderItens = $orderItens;
        }
        
        public function setCreatedAt($createdAt){
            $this->_createdAt = $createdAt;
        }

        public function getId() {
            return $this->_orderId;
        }
        public function getTotal() {
            return $this->_total;
        }
        public function getUserId(){
            return $this->_userId;
        }
        public function getStateId(){
            return $this->_stateId;
        }
        public function getCardOwner(){
            return $this->_cardOwnerName;
        }
        public function getExpirationDate(){
            return $this->_expirationDate;
        }
        public function getName(){
            return $this->_name;
        }
        public function getAddress(){
            return $this->_address;
        }
        public function getCreatedAt(){
            return $this->_createdAt;
        }
        public function getNeighborhood(){
            return $this->_neighborhood;
        }
        public function getCep(){
            return $this->_cep;
        }
        public function getCity(){
            return $this->_city;
        }
        public function getCpf(){
            return $this->_cpf;
        }
        public function getOrderItens(){
            return $this->_orderItens;
        }

        public function getOrderId(){
            return $this->_orderId;
        }
        public function getComplement(){
            return $this->_complement;
        }

        public function getTotalFormatted() {
            return "R$ ".  number_format($this->_total,2,",",".");
            
        }
        public function getCreateAtFormatted() {
            
            return date("d-m-Y H:i:s", strtotime( $this->_createdAt ) );
        }
    }
?>