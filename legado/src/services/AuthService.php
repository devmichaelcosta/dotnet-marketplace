<?php

namespace services;

class AuthService
{
    public static function isAuthorized($item): bool
    {
        $rolesPermitidas = array();
        if (!is_null($item[1]) && isset($item[1]) && $item[1] != "") {
            //deve estar logado
            // echo "rota tem acesso restrito<br>";
            $rolesPermitidas = explode(",", $item[1]);

            if (!isset($_SESSION["role"]) || is_null($_SESSION["role"])) {
                //echo "nao tem usuario logado";
                return false;
            } else {
                if (in_array($_SESSION["role"], $rolesPermitidas)) {
                    //echo "a role" . $_SESSION["role"] . "tem acesso";
                    return true;
                } else {

                    echo "a role" . $_SESSION["role"] . "nao tem acesso";
                    return false;
                }
            }
        } else {
            return true;
        }
    }
}
