<header>
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
        <div class="container">
            <a class="navbar-brand" title="Voltar para o site." href="/">
                <span class="logo-strong">L</span>oja<span class="logo-strong">W</span>hatever 
            </a>
            <button class="navbar-toggler" type="button" 
                    data-toggle="collapse" data-target="#navbarSupportedContent" 
                    aria-controls="navbarSupportedContent" aria-expanded="false" 
                    aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>

            <div class="collapse navbar-collapse" id="navbarSupportedContent">
                <?php 
                    if($_SESSION["role"] == "comum") 
                    {
                        require_once "./views/partials/nav-menu-default.php";
                    }
                    else if($_SESSION["role"] == "vendedor")
                    {
                        require_once "./views/partials/nav-menu-seller.php";
                    }
                    else
                    {
                        require_once "./views/partials/nav-menu-admin.php";
                    }
                ?>
               
                <div class="form-inline">
                    <span class="login-nav text-light ml-3">
                        <?php if (isset($_SESSION["userId"])) :?>
                            <i class="fa fa-user"></i> Olá, 
                            <a href="/admin/usuario/editar?id=<?php echo $_SESSION["userId"]?>" class="a-primary" >
                                <?php echo $_SESSION["userName"] ?>
                            </a> clique em 
                            <a href="/logout" class="a-primary" title="Sair no sistema" 
                                data-container="body"
                                data-toggle="tooltip" data-placement="body">Sair
                            </a> 
                            <br /> para fazer seu logout.
                        <?php endif;?>
                    </span>
                </div>
            </div>
        </div>
    </nav>
</header>