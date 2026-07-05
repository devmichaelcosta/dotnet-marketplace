<?php

namespace controllers\users;

use controllers\IBaseController;

class UserLogoutController implements IBaseController
{

    public function __construct($factory)
    {
    }

    public function proccessRequest(): void
    {
        if (isset($_SESSION["userId"])) {
            $_SESSION["userId"] = null;
            $_SESSION["role"] = null;
            $_SESSION["userName"] = null;
            $_SESSION["sellerId"] = null;
            header("location: /");
            exit;
        }
    }
}
