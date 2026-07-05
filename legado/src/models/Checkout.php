<?php
    namespace models;

    class Checkout
    {
        private $_carrinho;
        private $_states;
        private $_address;
        /**
         * Model responsavel pela pagina de checkout onde o usuario informa os dados de cartao de credito e endereço.
         */
        public function __construct($carrinho, $states,$address)
        {
            $this->_carrinho = $carrinho;
            $this->_states = $states;
            $this->_address = $address;
        }

        public function getCarrinho()
        {
            return $this->_carrinho;
        }

        public function getTotal()
        {
            return $this->_carrinho->getTotalFormatted();
        }

        public function getProducts()
        {
            return $this->_carrinho->getProducts();
        }

        public function getStates()
        {
            return $this->_states;
        }

        public function getAddress()
        {
            return $this->_address;
        }

        public function getCep(){ return $this->getAddress()->getCep();}
        public function getStreet(){ return $this->getAddress()->getStreet();}
        public function getNeighborhood(){ return $this->getAddress()->getNeighborhood();}
        public function getStateId(){ return $this->getAddress()->getStateId();}
        public function getCity(){ return $this->getAddress()->getCity();}
        public function getComplement(){ return $this->getAddress()->getComplement();}
    }