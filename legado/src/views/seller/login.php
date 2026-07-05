<!doctype html>
<html lang="en">

<head>
    <title>Loja Whatever - Sua loja de instrumentos músicais e acessórios</title>

    <!-- Meta tags -->
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <link href="https://fonts.googleapis.com/css?family=Roboto&display=swap" rel="stylesheet">

    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" integrity="sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T" crossorigin="anonymous">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css">
    <link rel="stylesheet" href="/assets/css/login.css">
    <link rel="stylesheet" href="/assets/libs/sweetAlert2/sweetalert2.min.css">
</head>

<body>
    <!-- cabeçalho e navegação -->
    <header>
        <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
            <div class="container-fluid">
                <a class="navbar-brand" href="/">
                    <span class="logo-strong">L</span>oja<span class="logo-strong">W</span>hatever</a>
            </div>
        </nav>
        <div class="spacer-header"></div>
    </header>
    <main>
        <div class="container-login">
            <div class="col-md-12">
                <form class="form-signin mt-5" id="form-login" method="post" action="/login-post">
                    <div class="card p-3">
                        <h1 class="h4 mb-3 font-weight-normal text-center ">
                            Olá! Para continuar, digite o seu login e senha
                        </h1>
                        <label for="login" class="sr-only">Login</label>
                        <input type="login" id="login" name="login" class="form-control" placeholder="Informe seu login" required="" autofocus="">

                        <label for="password" class="sr-only">Password</label>
                        <input type="password" id="password" name="password" class="form-control" placeholder="Informe sua senha" required="">

                        <button class="btn btn-lg btn-block mt-3 btn-outline-dark" data-loading-text="Processando, aguarde <i class='fa fa-circle-o-notch fa-spin'></i>" type="submit">
                            Entrar
                        </button>
                        <button class="btn btn-lg btn-block mt-1 btn-outline-dark" id="btn-register" data-loading-text="Processando, aguarde <i class='fa fa-circle-o-notch fa-spin'></i>" type="button">
                            Seja um vendedor
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </main>
    <footer class="mt-3 text-light"></footer>

    <!-- Optional JavaScript -->
    <script src="/assets/libs/jquery/jquery-3.4.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js" integrity="sha384-JjSmVgyd0p3pXB1rRibZUAYoIIy6OrQ6VrjIEaFf/nJGzIxFDsf4x0xIM+B07jRM" crossorigin="anonymous"></script>
    <script src="/assets/libs/sweetAlert2/sweetalert2.min.js"></script>
    <script src="/assets/js/bs4-extensions.js"></script>
    <script src="/assets/js/sweetAlertHelper.js"></script>
    <script src="/assets/js/models/LoginUsuario.js"></script>

    <script>
        let loginUsuario = null;
        $(function() {
            loginUsuario = new LoginUsuario();
        });
    </script>
</body>

</html>