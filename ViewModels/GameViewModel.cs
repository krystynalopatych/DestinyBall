using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DestinyBall.Models;
using System.Windows.Threading;

namespace DestinyBall.ViewModels
{
    public enum GamePhase
    {
        Idle,
        BallInFlight,
        Caught,
        Dropped,
        BotThinking,
        GameOver
    }

    public partial class GameViewModel : ObservableObject
    {
        private readonly GameModel _model;
        private readonly Random _rng = new();

        // ── Observable state ──────────────────────────────────────────────
        [ObservableProperty] private string _playerName = "";
        [ObservableProperty] private string _currentCategoryDisplay = "";
        [ObservableProperty] private string _statusMessage = "";
        [ObservableProperty] private string _throwWordInput = "";
        [ObservableProperty] private bool _canThrow = false;
        [ObservableProperty] private bool _isThrowerMode = false;
        [ObservableProperty] private bool _isCatcherMode = false;
        [ObservableProperty] private string _flightWord = "";
        [ObservableProperty] private bool _isWordVisible = false;
        [ObservableProperty] private double _timerProgress = 1.0;
        [ObservableProperty] private string _botFaceEmoji = "😐";
        [ObservableProperty] private string _botFaceMessage = "";
        [ObservableProperty] private GamePhase _currentPhase = GamePhase.Idle;
        [ObservableProperty] private bool _isBallFlying = false;
        [ObservableProperty] private bool _isGameOver = false;
        [ObservableProperty] private bool _throwingToBot = true;

        // Destiny display
        [ObservableProperty] private string _destName = "?";
        [ObservableProperty] private string _destAge = "?";
        [ObservableProperty] private string _destJob = "?";
        [ObservableProperty] private string _destHouse = "?";
        [ObservableProperty] private string _destCar = "?";
        [ObservableProperty] private string _destPartner = "?";
        [ObservableProperty] private string _destKids = "?";
        [ObservableProperty] private string _destHobby = "?";

        [ObservableProperty] private string _botDestName = "?";
        [ObservableProperty] private string _botDestAge = "?";
        [ObservableProperty] private string _botDestJob = "?";
        [ObservableProperty] private string _botDestHouse = "?";
        [ObservableProperty] private string _botDestCar = "?";
        [ObservableProperty] private string _botDestPartner = "?";
        [ObservableProperty] private string _botDestKids = "?";
        [ObservableProperty] private string _botDestHobby = "?";

        // Internal state
        // Round structure: for each of 8 categories, we do 2 throws:
        //   Step A: determine BOT's destiny  (player throws to bot if Thrower; else auto-throw)
        //   Step B: determine PLAYER's destiny (bot throws to player if Catcher; else auto-throw)
        private int _categoryIndex = 0;
        private bool _stepA = true; // true = setting bot's destiny, false = setting player's
        private string _currentWord = "";
        private bool _playerDodged = false;

        private static readonly GameCategory[] Categories =
        {
            GameCategory.Name, GameCategory.Age, GameCategory.Job, GameCategory.House,
            GameCategory.Car, GameCategory.Partner, GameCategory.Kids, GameCategory.Hobby
        };

        private GameCategory CurrentCategory => Categories[_categoryIndex];

        // Events
        public event Action? StartThrowAnimation;

        public GameViewModel(GameModel model)
        {
            _model = model;
            PlayerName = model.PlayerName;
            IsThrowerMode = model.PlayerRole == PlayerRole.Thrower;
            IsCatcherMode = model.PlayerRole == PlayerRole.Catcher;
            UpdateCategoryDisplay();
            Delay(400, BeginStep);
        }

        // ─────────────────────────────────────────────────────────────────
        // Step management
        // ─────────────────────────────────────────────────────────────────

        private void BeginStep()
        {
            if (_categoryIndex >= 8) { EndGame(); return; }
            UpdateCategoryDisplay();

            if (_stepA)
                BeginStepA_BotDestiny();
            else
                BeginStepB_PlayerDestiny();
        }

        /// <summary>Step A: Throw to determine BOT's destiny.</summary>
        private void BeginStepA_BotDestiny()
        {
            ThrowingToBot = true;
            if (IsThrowerMode)
            {
                // Player types and throws
                StatusMessage = $"✏️  Введи слово и брось боту! ({WordLists.GetCategoryDisplayName(CurrentCategory)})";
                CanThrow = true;
                BotFaceEmoji = "😏";
                BotFaceMessage = "Жду...";
                CurrentPhase = GamePhase.Idle;
            }
            else
            {
                // Catcher mode: auto-throw to bot (player has no role here)
                StatusMessage = "Автоматический бросок боту...";
                CanThrow = false;
                BotFaceEmoji = "🙂";
                BotFaceMessage = "Авто-бросок!";
                CurrentPhase = GamePhase.BotThinking;
                Delay(700, () => AutoThrowToBot());
            }
        }

        /// <summary>Step B: Throw to determine PLAYER's destiny.</summary>
        private void BeginStepB_PlayerDestiny()
        {
            ThrowingToBot = false;
            if (IsCatcherMode)
            {
                // Main mechanic: bot throws, player dodges or catches
                StatusMessage = "🚨  Готовься! Жми мышку чтобы уклониться!";
                CanThrow = false;
                CurrentPhase = GamePhase.BotThinking;
                Delay(800 + _rng.Next(600), ExecuteBotThrow);
            }
            else
            {
                // Thrower mode: auto-throw from bot to player
                StatusMessage = "Бот возвращает мяч...";
                CanThrow = false;
                BotFaceEmoji = "🙂";
                BotFaceMessage = "Возвращаю!";
                CurrentPhase = GamePhase.BotThinking;
                Delay(700, () => AutoThrowToPlayer());
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // Throw: player throws to bot (Thrower mode, Step A)
        // ─────────────────────────────────────────────────────────────────

        [RelayCommand]
        private void Throw()
        {
            if (!CanThrow || string.IsNullOrWhiteSpace(ThrowWordInput)) return;

            _currentWord = ThrowWordInput.Trim();
            ThrowWordInput = "";
            CanThrow = false;
            ThrowingToBot = true;

            FlightWord = _currentWord;
            IsWordVisible = true;
            IsBallFlying = true;
            CurrentPhase = GamePhase.BallInFlight;
            BotFaceEmoji = "👀";
            BotFaceMessage = "Лечи-и-ит!";
            StatusMessage = $"Бросаешь: «{_currentWord}»!";

            StartThrowAnimation?.Invoke();

            Delay(1400, () =>
            {
                IsBallFlying = false;
                IsWordVisible = false;
                FlightWord = "";

                bool botCatches = _rng.NextDouble() < 0.70;
                if (botCatches)
                {
                    SetBotDestiny(CurrentCategory, _currentWord);
                    BotFaceEmoji = "😄";
                    BotFaceMessage = GetBotReaction(_currentWord);
                    StatusMessage = $"✅  Бот поймал «{_currentWord}»!";
                    CurrentPhase = GamePhase.Caught;
                    Delay(1400, AdvanceStep);
                }
                else
                {
                    BotFaceEmoji = "😅";
                    BotFaceMessage = "Упс! Уронил!";
                    StatusMessage = "😬  Бот уронил! Бросай снова...";
                    CurrentPhase = GamePhase.Dropped;
                    Delay(1200, () =>
                    {
                        CanThrow = true;
                        CurrentPhase = GamePhase.Idle;
                        StatusMessage = $"Попробуй ещё раз! ({WordLists.GetCategoryDisplayName(CurrentCategory)})";
                    });
                }
            });
        }

        // ─────────────────────────────────────────────────────────────────
        // Auto-throw to bot (Catcher mode, Step A)
        // ─────────────────────────────────────────────────────────────────

        private void AutoThrowToBot()
        {
            _currentWord = GetRandomWord(CurrentCategory);
            ThrowingToBot = true;
            bool botCatches = _rng.NextDouble() < 0.70;

            FlightWord = _currentWord;
            IsWordVisible = true;
            IsBallFlying = true;
            CurrentPhase = GamePhase.BallInFlight;
            BotFaceEmoji = "👀";
            BotFaceMessage = "Лечит!";
            StartThrowAnimation?.Invoke();

            Delay(1400, () =>
            {
                IsBallFlying = false;
                IsWordVisible = false;
                FlightWord = "";

                if (botCatches)
                {
                    SetBotDestiny(CurrentCategory, _currentWord);
                    BotFaceEmoji = "😄";
                    BotFaceMessage = GetBotReaction(_currentWord);
                    StatusMessage = $"✅  Бот поймал «{_currentWord}»!";
                    CurrentPhase = GamePhase.Caught;
                    Delay(1200, AdvanceStep);
                }
                else
                {
                    BotFaceEmoji = "😅";
                    BotFaceMessage = "Уронил!";
                    StatusMessage = "Бот уронил! Повтор...";
                    CurrentPhase = GamePhase.Dropped;
                    Delay(1000, () => AutoThrowToBot());
                }
            });
        }

        // ─────────────────────────────────────────────────────────────────
        // Auto-throw to player (Thrower mode, Step B)
        // ─────────────────────────────────────────────────────────────────

        private void AutoThrowToPlayer()
        {
            _currentWord = GetRandomWord(CurrentCategory);
            ThrowingToBot = false;

            FlightWord = _currentWord;
            IsWordVisible = true;
            IsBallFlying = true;
            CurrentPhase = GamePhase.BallInFlight;
            BotFaceEmoji = "😊";
            BotFaceMessage = "Лови!";
            StatusMessage = $"Бот бросает тебе: «{_currentWord}»";
            StartThrowAnimation?.Invoke();

            Delay(1400, () =>
            {
                IsBallFlying = false;
                IsWordVisible = false;
                FlightWord = "";
                // Auto-catch: player always catches in thrower mode for their own destiny
                SetPlayerDestiny(CurrentCategory, _currentWord);
                BotFaceEmoji = "🤗";
                BotFaceMessage = "Поймал!";
                StatusMessage = $"🎯  Твоя судьба: «{_currentWord}»!";
                CurrentPhase = GamePhase.Caught;
                Delay(1400, AdvanceStep);
            });
        }

        // ─────────────────────────────────────────────────────────────────
        // Bot throws to player (Catcher mode, Step B) — CORE MECHANIC
        // ─────────────────────────────────────────────────────────────────

        private void ExecuteBotThrow()
        {
            _currentWord = GetRandomWord(CurrentCategory);
            _playerDodged = false;
            ThrowingToBot = false;

            BotFaceEmoji = "😈";
            BotFaceMessage = "Кидаю!";
            StatusMessage = "💥  МЯЧ ЛЕТИТ! Жми мышку чтобы уклониться!";

            FlightWord = "???";
            IsWordVisible = false;
            IsBallFlying = true;
            CurrentPhase = GamePhase.BallInFlight;
            TimerProgress = 1.0;

            _dodgeWindowOpen = true;
            StartThrowAnimation?.Invoke();

            // Reveal word halfway through (~0.65s into 1.5s flight)
            Delay(650, () =>
            {
                if (CurrentPhase == GamePhase.BallInFlight && !_playerDodged)
                {
                    FlightWord = _currentWord;
                    IsWordVisible = true;
                    BotFaceEmoji = "😏";
                    BotFaceMessage = "Видишь?! 😂";
                }
            });

            // Drain timer bar
            StartTimerDrain(1500, OnFlightTimerExpired);
        }

        private void OnFlightTimerExpired()
        {
            if (CurrentPhase != GamePhase.BallInFlight) return;
            _dodgeWindowOpen = false;
            IsBallFlying = false;
            IsWordVisible = false;
            FlightWord = "";
            TimerProgress = 0;

            if (_playerDodged)
            {
                BotFaceEmoji = "😤";
                BotFaceMessage = "Трус!!";
                StatusMessage = "💨  Уклонился! Бот злится...";
                CurrentPhase = GamePhase.Dropped;
                Delay(1200, () =>
                {
                    CurrentPhase = GamePhase.BotThinking;
                    Delay(800 + _rng.Next(400), ExecuteBotThrow);
                });
            }
            else
            {
                // Player caught
                SetPlayerDestiny(CurrentCategory, _currentWord);
                BotFaceEmoji = "😂";
                BotFaceMessage = "ХАХА! Попался!";
                StatusMessage = $"😵  Поймал! Твоя судьба: «{_currentWord}»!";
                CurrentPhase = GamePhase.Caught;
                FlightWord = _currentWord;
                IsWordVisible = true;
                Delay(1800, () =>
                {
                    IsWordVisible = false;
                    FlightWord = "";
                    AdvanceStep();
                });
            }
        }

        // Called by View when player clicks mouse during flight
        // Set to true during the dodge window (bot throwing to player)
        private bool _dodgeWindowOpen = false;

        public void OnPlayerClick()
        {
            if (!_dodgeWindowOpen) return;

            _dodgeWindowOpen = false;
            _playerDodged = true;
            _timerBarTimer?.Stop();
            IsBallFlying = false;
            IsWordVisible = false;
            CurrentPhase = GamePhase.Dropped;
            BotFaceEmoji = "😤";
            BotFaceMessage = "Уклонился!!";
            StatusMessage = "💨  Увернулся! Бот в ярости...";
            FlightWord = "";
            TimerProgress = 0;
            Delay(1200, () =>
            {
                CurrentPhase = GamePhase.BotThinking;
                Delay(800 + _rng.Next(400), ExecuteBotThrow);
            });
        }

        // ─────────────────────────────────────────────────────────────────
        // Step advance
        // ─────────────────────────────────────────────────────────────────

        private void AdvanceStep()
        {
            if (_stepA)
            {
                // Done with bot's destiny, now do player's destiny
                _stepA = false;
                BeginStep();
            }
            else
            {
                // Done with both, advance category
                _stepA = true;
                _categoryIndex++;
                if (_categoryIndex >= 8)
                    EndGame();
                else
                    BeginStep();
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // Timer bar
        // ─────────────────────────────────────────────────────────────────

        private DispatcherTimer? _timerBarTimer;
        private Action? _timerExpiredCallback;

        private void StartTimerDrain(double durationMs, Action onExpired)
        {
            _timerExpiredCallback = onExpired;
            double elapsed = 0;
            _timerBarTimer?.Stop();
            _timerBarTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(30) };
            _timerBarTimer.Tick += (s, e) =>
            {
                elapsed += 30;
                TimerProgress = Math.Max(0, 1.0 - (elapsed / durationMs));
                if (elapsed >= durationMs)
                {
                    _timerBarTimer?.Stop();
                    _timerExpiredCallback?.Invoke();
                }
            };
            _timerBarTimer.Start();
        }

        // ─────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────

        private void Delay(double ms, Action action)
        {
            var t = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(ms) };
            t.Tick += (s, e) => { t.Stop(); action(); };
            t.Start();
        }

        private string GetRandomWord(GameCategory cat)
        {
            var words = WordLists.GetWordsForCategory(cat);
            return words[_rng.Next(words.Length)];
        }

        private string GetBotReaction(string word)
        {
            bool cringe = word.Length > 8 || word.Contains("крематор") || word.Contains("пакет") ||
                          word.Contains("Жора") || word.Contains("призрак") || word.Contains("0 но");
            return cringe ? "ХАХА!! 😂" : "Поймал!";
        }

        private void UpdateCategoryDisplay()
        {
            if (_categoryIndex < 8)
                CurrentCategoryDisplay = WordLists.GetCategoryDisplayName(Categories[_categoryIndex]);
        }

        private void SetPlayerDestiny(GameCategory cat, string word)
        {
            _model.PlayerDestiny[cat] = word;
            UpdatePlayerDisplay();
        }

        private void SetBotDestiny(GameCategory cat, string word)
        {
            _model.BotDestiny[cat] = word;
            UpdateBotDisplay();
        }

        private void UpdatePlayerDisplay()
        {
            string Get(GameCategory k) => _model.PlayerDestiny.TryGetValue(k, out var v) ? v : "?";
            DestName = Get(GameCategory.Name);
            DestAge = Get(GameCategory.Age);
            DestJob = Get(GameCategory.Job);
            DestHouse = Get(GameCategory.House);
            DestCar = Get(GameCategory.Car);
            DestPartner = Get(GameCategory.Partner);
            DestKids = Get(GameCategory.Kids);
            DestHobby = Get(GameCategory.Hobby);
        }

        private void UpdateBotDisplay()
        {
            string Get(GameCategory k) => _model.BotDestiny.TryGetValue(k, out var v) ? v : "?";
            BotDestName = Get(GameCategory.Name);
            BotDestAge = Get(GameCategory.Age);
            BotDestJob = Get(GameCategory.Job);
            BotDestHouse = Get(GameCategory.House);
            BotDestCar = Get(GameCategory.Car);
            BotDestPartner = Get(GameCategory.Partner);
            BotDestKids = Get(GameCategory.Kids);
            BotDestHobby = Get(GameCategory.Hobby);
        }

        private void EndGame()
        {
            CurrentPhase = GamePhase.GameOver;
            IsGameOver = true;
            StatusMessage = "🎉  ИГРА ОКОНЧЕНА! Судьба решена!";
            BotFaceEmoji = "🎉";
            BotFaceMessage = "Всё решено!";
        }

        public GameModel GetModel() => _model;
    }
}
