<?php
    namespace models;

    class Seller
    {
        private $_sellerId;
        private $_userId;
        private $_name; 
        private $_company;
        private $_lastName;
        private $_fantasyName; 
        
        private $_login;
        private $_age;
        
        private $_cpf;
        private $_email;
        private $_dateOfBirth;
        private $_webSite;
        private $_cnpj;
        private $_branchOfActivity;
        

        public function __construct(
            $sellerId, $name, $lastName, $company, $fantasyName,  $login, $userId)
        {
            $this->_sellerId = $sellerId;
            $this->_name = $name;
            $this->_lastName = $lastName;
            $this->_company = $company;
            $this->_fantasyName = $fantasyName;
            $this->_login = $login;
            $this->_userId = $userId;
        }

        public function getLogin()
        {
            return $this->_login;
        }

        public function getSellerId()
        {
            return $this->_sellerId;
        }

        public function getUserId()
        {
            return $this->_userId;
        }

        public function getName()
        {
            return $this->_name;
        }
        
        public function getLastName()
        {
            return $this->_lastName;
        }

        public function getFantasyName()
        {
            return $this->_fantasyName;
        }

        
        public function getAge(){ return $this->_age; }
        public function getCpf(){ return $this->_cpf; }
        public function getDateOfBirth(){ return $this->_dateOfBirth; }
        public function getWebsite(){ return $this->_webSite; }
        public function getCompany(){ return $this->_company; }
        public function getBranchOfActivity(){ return $this->_branchOfActivity; }
        public function getCnpj(){ return $this->_cnpj; }
        public function getEmail(){ return $this->_email; }

        public function setCpf($cpf){ $this->_cpf = $cpf;}
        public function setAge($age){ $this->_age = $age;}
        public function setEmail($email){ $this->_email = $email;}
        public function setDateOfBirth($dataOfBirth){ $this->_dateOfBirth = $dataOfBirth;}
        public function setWebsite($webSite){ $this->_webSite = $webSite;}
        public function setCompany($company){ $this->_company = $company;}
        public function setBranchOfActivity($branchOfActivity){ $this->_branchOfActivity = $branchOfActivity;}
        public function setCnpj($cnpj){ $this->_cnpj = $cnpj;}
        public function setUserId($userId){ $this->_userId = $userId;}
        public function setFantasyName($fantasyName){ $this->_fantasyName = $fantasyName;}


        public function isValid() : bool
        {
            return true;

        }
    }
?>