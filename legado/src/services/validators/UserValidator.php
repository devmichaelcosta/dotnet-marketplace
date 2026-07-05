<?php

namespace services\validators;

use models\User;

class UserValidator
{
    private $errors = array();
    private $user;

    public function __construct(User $user)
    {
        $this->user = $user;
    }

    public function errors()
    {
        return $this->errors;
    }

    public function isValid()
    {
        $this->validateName();
        $this->validatePassword();
        $this->validateLogin();

        return count($this->errors) === 0;
    }

    private function validateName()
    {
        if ($this->user->getName() === '')
            $this->errors['name'] = 'Informe o nome.';
    }

    private function validatePassword()
    {
        if ($this->user->getPassword() === '')
            $this->errors['password'] = 'Informe a senha.';
    }

    private function validateLogin()
    {
        if ($this->user->getLogin() === '')
            $this->errors['login'] = 'Informe o login.';
    }
}
