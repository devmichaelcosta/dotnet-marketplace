<?php
    namespace models;

    class SellerCreateViewModel
    {
        private $_states; 
        
        public function __construct($states)
        {
            $this->_states = $states;
        }

        public function getStates()
        {
            return $this->_states;
        }
    }