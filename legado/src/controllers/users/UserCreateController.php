<?php
    namespace controllers\users;    
    use controllers\IBaseController;

    class UserCreateController implements IBaseController
    {
        public function __construct($factory)
        {
            
        }
        
        public function proccessRequest() : void
        {
            require "views/admin/users/cadastrar-usuario.php";
        }
    }
