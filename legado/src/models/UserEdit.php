<?php
    namespace models;


    class UserEdit
    {
        private $errors = array();
        private $userId;
        private $login;
        private $name;
        private $role;
        private $_addresses;
        private $_cpf;
        private $_lastName;

        public function __construct($userId,$name,$login,$role,$cpf,$lastName)
        {
            $this->userId = $userId;
            $this->login = $login;
            $this->name = $name;
            $this->role = $role;
            $this->_cpf = $cpf;
            $this->_lastName = $lastName;
            $this->_addresses = array();
        }

        public function getUserId()
        {
            return $this->userId;
        }

        public function getLogin()
        {
            return $this->login;
        }

        public function getName()
        {
            return $this->name;
        }
        public function getLastName()
        {
            return $this->_lastName;
        }
        public function getRole()
        {
            return $this->role;
        }
        public function getCpf()
        {
            return $this->_cpf;
        }
        public function getAddresses()
        {
            return $this->_addresses;
        }
        public function setAddresses($addresses)
        {
            $this->_addresses = $addresses;
        }

        public function errors()
        {
            return $this->errors;
        }

        public function isValid()
        {
            $this->validateName();
            $this->validateLogin();
            $this->validateRole();
            return count($this->errors) === 0;
        }

        private function validateName()
        {
            if ($this->getName() === '')
                $this->errors['name'] = 'Informe o nome.';
        }

        private function validateLogin()
        {
            if ($this->getLogin() === '')
                $this->errors['login'] = 'Informe o login.';
        }

        private function validateRole()
        {
            if ($this->getRole() === '')
                $this->errors['role'] = 'Informe o tipo de usuário.';
        }
    }

?>