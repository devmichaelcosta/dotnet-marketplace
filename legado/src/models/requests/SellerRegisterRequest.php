<?php   

    namespace models\requests;
    
    class SellerRegisterRequest 
    {
        private $_name;
        private $_lastName;
        private $_login;
        private $_password;
        private $_errors;

        /**
         * Class constructor.
         */
        public function __construct(string $name, string $lastName, string $login, string $password)
        {
            $this->_name = $name;
            $this->_lastName = $lastName;
            $this->_login = $login;
            $this->_password = $password;
            $this->_errors = array();
        }
        
        public function getErrors()
        {
            return $this->_errors;
        }

        public function isValid()
        {
            $this->validateName();
            $this->validatePassword();
            $this->validateLogin();
            $this->validateLastName();

            return count($this->_errors) === 0;
        }

        private function validateName()
        {
            if ($this->_name === '')
                $this->_errors['name'] = 'Informe o nome.';
        }

        private function validateLastName()
        {
            if ($this->_lastName === '')
                $this->_errors['lastName'] = 'Informe o sobrenome.';
        }

        private function validatePassword()
        {
            if ($this->_password === '')
                $this->_errors['password'] = 'Informe a senha.';
        }

        private function validateLogin()
        {
            if ($this->_login === '')
                $this->_errors['login'] = 'Informe o login.';
        }

        public function getLogin(){
            return $this->_login;
        }
        public function getPassword(){
            return $this->_password;
        }
        public function getName(){
            return $this->_name;
        }
        public function getLastName(){
            return $this->_lastName;
        }
    }