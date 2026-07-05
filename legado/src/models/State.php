<?php

    namespace models;

    class State
    {
        private $_stateId;
        private $_name;        
        private $_stateAbreviattion;
        
        public function __construct($id, string $name, string $stateAbreviattion)
        {
            $this->_stateId = $id;
            $this->_name = $name;
            $this->_stateAbreviattion = $stateAbreviattion;
        }
        
        public function getName()
        {
            return  $this->_name;
        }
        
        public function getId()
        {
            return  $this->_stateId;
        }
        
        public function getStateAbreviattion()
        {
            return  $this->_stateAbreviattion;
        }

        
    }
?>