<?php
    namespace models;

    class SellerEditViewModel
    {
        private $_states; 
        private $_seller;
        private $_address;
        private $_user;
        public function __construct($seller, $states,$address,$user)
        {
            $this->_states = $states;
            $this->_seller = $seller;
            $this->_address = $address;
            $this->_user = $user;
        }

        public function getStates()
        {
            return $this->_states;
        }

        public function getSellerId() { return $this->_seller->getSellerId(); }
        public function getName() { return $this->_seller->getName(); }
        public function getLastName() { return $this->_seller->getLastName(); }
        public function getEmail(){ return $this->_seller->getEmail(); }
        public function getFantasyName() { return $this->_seller->getFantasyName(); }
        public function getAge(){ return $this->_seller->getAge(); }
        public function getCpf(){ return $this->_seller->getCpf(); }
        public function getDateOfBirth(){ return $this->_seller->getDateOfBirth(); }
        public function getWebsite(){ return $this->_seller->getWebsite(); }
        public function getCompany(){ return $this->_seller->getCompany(); }
        public function getBranchOfActivity(){ return $this->_seller->getBranchOfActivity(); }
        public function getCnpj(){ return $this->_seller->getCnpj(); }
        

        public function getStreet(){ return is_null($this->_address) ? "" : $this->_address->getStreet(); }
        public function getNeighborhood(){ return is_null($this->_address) ? "" : $this->_address->getNeighborhood();}
        public function getStateId(){ return is_null($this->_address) ? "" : $this->_address->getStateId();}
        public function getCity(){ return is_null($this->_address) ? "" : $this->_address->getCity();}
        public function getCep(){ return is_null($this->_address) ? "" :  $this->_address->getCep();}
        public function getComplement(){return is_null($this->_address) ? "" : $this->_address->getComplement();}
    }