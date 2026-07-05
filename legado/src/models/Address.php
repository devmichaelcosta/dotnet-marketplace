<?php
    namespace models;

    class Address
    {
        private $_addressId;
        private $_userId;
        private $_street;
        private $_cep;
        private $_neighborhood;
        private $_city;
        private $_stateid;
        private $_complement;

        /**
         * Class constructor.
         */
        public function __construct(
            $addressId, $userId, $street, $cep, $neighborhood, $city, $stateId, $complement)
        {
            $this->_addressId = $addressId;
            $this->_userId = $userId;
            $this->_street = $street;
            $this->_cep = $cep;
            $this->_neighborhood = $neighborhood;
            $this->_city = $city;
            $this->_stateid = $stateId;
            $this->_complement = $complement;
        }

        public function getAddressId()
        {
            return $this->_addressId;
        }
        public function getUserId()
        {
            return $this->_userId;
        }
        public function getStreet()
        {
            return $this->_street;
        }
        public function getCep()
        {
            return $this->_cep;
        }
        public function getNeighborhood()
        {
            return $this->_neighborhood;
        }
        public function getCity()
        {
            return $this->_city;
        }
        public function getStateId()
        {
            return $this->_stateid;
        }
        public function getComplement()
        {
            return $this->_complement;
        }

        public function setUserId($userId)
        {
            $this->_userId = $userId;
        }

        public function setStreet($street){ $this->_street = $street; }
        public function setCep($cep){ $this->_cep = $cep; }
        public function setNeighborhood($neighborhood){ $this->_neighborhood = $neighborhood; }
        public function setCity($city){ $this->_city = $city; }
        public function setStateId($stateId){ $this->_stateId = $stateId; }
        public function setComplement($complement){ $this->_complement = $complement; }
    }