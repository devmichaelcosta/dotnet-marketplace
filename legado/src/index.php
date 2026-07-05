<?php

use services\SessionService;
use services\AuthService;

require_once './infra/configs/autoload.php';

$basePath = __DIR__;

ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);

//abstração do banco de dados...
$factory = new infra\MySqlRepositoryFactory();

//iniciando sessao
$sessionService = new SessionService();
$sessionService->start();

$caminho =  "/";

if (isset($_SERVER["PATH_INFO"]))
    $caminho =  $_SERVER["PATH_INFO"];

$rotas = require __DIR__ . './infra/configs/router.php';

if (!array_key_exists($caminho, $rotas)) {
    http_response_code(404);
    exit();
}

//verificando se o usuario tem acesso a rota requisitada
if (!AuthService::isAuthorized($rotas[$caminho]))
    header('Location: /login');

//fazendo um "de para" de rota para o Controller alvo...
$controllerAlvo = $rotas[$caminho][0];

// echo "index > processar request<br/>";
$controlador = new $controllerAlvo($factory);
$controlador->proccessRequest();
