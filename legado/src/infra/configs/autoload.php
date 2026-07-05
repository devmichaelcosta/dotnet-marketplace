<?php
    //sempre o que solicitamos uma classes o autoloader gerencia isso...
    //dessa forma nao precisamos ficar usando require ou include,
    //porem necessitamos ter os namespaces corretos...

    function load($namespace)
    {
        $namespace = str_replace("\\" , "/", $namespace);
        
        //voltando um diretorio pois o autoload esta em /infra...
        $caminhoAbsoluto = __DIR__ . "../../../" . $namespace . ".php";
        return include_once $caminhoAbsoluto;
    }
    spl_autoload_register(__NAMESPACE__ . "\load");
?>